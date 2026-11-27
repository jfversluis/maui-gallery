#!/usr/bin/env python3
"""Uploads the App Store and Mac App Store listing to App Store Connect.

Screenshots come from eng/release/app-store/screenshots/<locale>/<device>/*.png and texts from
eng/release/app-store/metadata/<locale>/*.txt. They go to the iOS and macOS versions that are being prepared (or were
rejected), replacing the screenshots and texts there. Files that don't exist are left alone in App Store Connect.

  app-store-listing.py --check [--screenshots] [--metadata]   validate the files, no credentials needed
  app-store-listing.py [--screenshots] [--metadata]           upload

Screenshots are replaced per device: the old ones are deleted first, so if an upload fails, run it again.

Uploading reads APP_STORE_CONNECT_KEY_ID, APP_STORE_CONNECT_ISSUER_ID, APP_STORE_CONNECT_PRIVATE_KEY (the contents of
the .p8 key) and APP_STORE_APP_ID from the environment. Only the Python standard library and the openssl command are
used. Screenshots use the app screenshot set resources of the App Store Connect API (deprecated in favor of the App
Asset Library, but still served). See docs/releasing.md.
"""

import argparse
import base64
import hashlib
import json
import os
import struct
import subprocess
import sys
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request
from pathlib import Path

API = "https://api.appstoreconnect.apple.com"
ROOT = Path(__file__).resolve().parent / "app-store"

# Screenshot folder: (platform, display type, accepted sizes). iPhone and iPad sizes also work in landscape.
DEVICES = {
    "iphone-6.9": ("IOS", "APP_IPHONE_67", {(1320, 2868), (1290, 2796), (1260, 2736)}),
    "ipad-13": ("IOS", "APP_IPAD_PRO_3GEN_129", {(2064, 2752), (2048, 2732)}),
    "mac": ("MAC_OS", "APP_DESKTOP", {(2880, 1800), (2560, 1600), (1440, 900), (1280, 800)}),
}
ROTATES = {"iphone-6.9", "ipad-13"}
MAX_SCREENSHOTS = 10

# Metadata file: (resource, attribute, maximum length; keywords count UTF-8 bytes).
FIELDS = {
    "name.txt": ("appInfoLocalization", "name", 30),
    "subtitle.txt": ("appInfoLocalization", "subtitle", 30),
    "privacy_policy_url.txt": ("appInfoLocalization", "privacyPolicyUrl", None),
    "description.txt": ("appStoreVersionLocalization", "description", 4000),
    "keywords.txt": ("appStoreVersionLocalization", "keywords", 100),
    "promotional_text.txt": ("appStoreVersionLocalization", "promotionalText", 170),
    "marketing_url.txt": ("appStoreVersionLocalization", "marketingUrl", None),
    "support_url.txt": ("appStoreVersionLocalization", "supportUrl", None),
    # Not accepted for an app's first version.
    "whats_new.txt": ("appStoreVersionLocalization", "whatsNew", 4000),
    "copyright.txt": ("appStoreVersion", "copyright", None),
}

EDITABLE_STATES = {"PREPARE_FOR_SUBMISSION", "DEVELOPER_REJECTED", "REJECTED", "METADATA_REJECTED", "INVALID_BINARY"}


def error(message):
    print(f"::error::{message}")


class Problems(Exception):
    pass


def png_size(path):
    content = Path(path).read_bytes()
    header = content[:26]
    if header[:8] != b"\x89PNG\r\n\x1a\n" or content[-8:-4] != b"IEND":
        raise Problems(f"{path} isn't a complete PNG.")
    width, height, _, color_type = struct.unpack(">IIBB", header[16:26])
    if color_type in (4, 6):
        raise Problems(f"{path} has an alpha channel, which App Store Connect rejects.")
    return width, height


def collect_screenshots():
    """Returns {(locale, folder): [files]} after checking every file."""
    problems, result = [], {}
    base = ROOT / "screenshots"
    for locale_dir in sorted(p for p in base.iterdir() if p.is_dir()) if base.exists() else []:
        for device_dir in sorted(p for p in locale_dir.iterdir() if p.is_dir()):
            if device_dir.name not in DEVICES:
                problems.append(f"{device_dir}: unknown device folder; use one of {', '.join(DEVICES)}.")
                continue
            sizes = DEVICES[device_dir.name][2]
            files = sorted(device_dir.glob("*.png"))
            if not 1 <= len(files) <= MAX_SCREENSHOTS:
                problems.append(f"{device_dir}: needs 1 to {MAX_SCREENSHOTS} screenshots, found {len(files)}.")
            for file in files:
                try:
                    size = png_size(file)
                except Problems as problem:
                    problems.append(str(problem))
                    continue
                if size not in sizes and not (device_dir.name in ROTATES and size[::-1] in sizes):
                    problems.append(f"{file}: {size[0]}x{size[1]} isn't a {device_dir.name} screenshot size.")
            result[(locale_dir.name, device_dir.name)] = files
    if problems:
        raise Problems("\n".join(problems))
    return result


def collect_metadata():
    """Returns {locale: {file name: text}} after checking every file."""
    problems, result = [], {}
    base = ROOT / "metadata"
    for locale_dir in sorted(p for p in base.iterdir() if p.is_dir()) if base.exists() else []:
        texts = {}
        for file in sorted(locale_dir.glob("*.txt")):
            if file.name not in FIELDS:
                problems.append(f"{file}: unknown field; use one of {', '.join(FIELDS)}.")
                continue
            text = file.read_text(encoding="utf-8").strip()
            limit = FIELDS[file.name][2]
            length = len(text.encode("utf-8")) if file.name == "keywords.txt" else len(text)
            if not text:
                problems.append(f"{file}: empty.")
            elif limit and length > limit:
                problems.append(f"{file}: {length} characters, the limit is {limit}.")
            texts[file.name] = text
        result[locale_dir.name] = texts
    if problems:
        raise Problems("\n".join(problems))
    return result


class AppStoreConnect:
    def __init__(self, key_id, issuer_id, private_key):
        self.key_id, self.issuer_id = key_id, issuer_id
        self.key_file = tempfile.NamedTemporaryFile("w", suffix=".p8", delete=False)
        self.key_file.write(private_key.strip() + "\n")
        self.key_file.close()
        os.chmod(self.key_file.name, 0o600)
        self.token, self.token_time = None, 0

    def close(self):
        os.unlink(self.key_file.name)

    def _jwt(self):
        # A token lasts 20 minutes at most; make a new one every 10.
        if self.token and time.time() - self.token_time < 600:
            return self.token
        now = int(time.time())

        def encode(value):
            raw = value if isinstance(value, bytes) else json.dumps(value, separators=(",", ":")).encode()
            return base64.urlsafe_b64encode(raw).rstrip(b"=")

        signing_input = encode({"alg": "ES256", "kid": self.key_id, "typ": "JWT"}) + b"." + encode(
            {"iss": self.issuer_id, "iat": now, "exp": now + 1200, "aud": "appstoreconnect-v1"})
        der = subprocess.run(["openssl", "dgst", "-sha256", "-sign", self.key_file.name], input=signing_input,
                             capture_output=True, check=True).stdout
        self.token = (signing_input + b"." + encode(der_to_raw_signature(der))).decode()
        self.token_time = now
        return self.token

    def request(self, method, path, body=None, params=None):
        url = path if path.startswith("https://") else API + path
        if params:
            url += "?" + urllib.parse.urlencode(params)
        data = json.dumps(body).encode() if body is not None else None
        request = urllib.request.Request(url, data=data, method=method)
        request.add_header("Authorization", f"Bearer {self._jwt()}")
        if data is not None:
            request.add_header("Content-Type", "application/json")
        try:
            with urllib.request.urlopen(request, timeout=120) as response:
                content = response.read()
        except urllib.error.HTTPError as failure:
            detail = failure.read().decode(errors="replace")
            try:
                detail = "; ".join(f"{e.get('title')}: {e.get('detail')}" for e in json.loads(detail)["errors"])
            except (ValueError, KeyError, TypeError):
                pass
            raise Problems(f"{method} {path} failed with {failure.code}: {detail}") from None
        return json.loads(content) if content else None

    def get_all(self, path, params=None):
        items, page = [], self.request("GET", path, params=dict(params or {}, limit="200"))
        while True:
            items.extend(page["data"])
            following = page.get("links", {}).get("next")
            if not following:
                return items
            page = self.request("GET", following)


def der_to_raw_signature(der):
    """openssl writes ECDSA signatures as DER; JWT wants r and s as two 32-byte big-endian numbers."""
    index = 3 if der[1] & 0x80 else 2
    raw = b""
    for _ in range(2):
        if der[index] != 0x02:
            raise ValueError("Unexpected ECDSA signature format.")
        length = der[index + 1]
        raw += int.from_bytes(der[index + 2:index + 2 + length], "big").to_bytes(32, "big")
        index += 2 + length
    return raw


def state_of(resource):
    # Versions report appVersionState and app information reports state; appStoreState is the deprecated older field.
    attributes = resource["attributes"]
    field = "appVersionState" if resource.get("type") == "appStoreVersions" else "state"
    current = attributes.get(field)
    return current or attributes.get("appStoreState")


def editable_version(api, app_id, platform):
    """Returns the version being prepared, and whether it's the platform's first one."""
    versions = api.get_all(f"/v1/apps/{app_id}/appStoreVersions", {"filter[platform]": platform})
    for version in versions:
        if state_of(version) in EDITABLE_STATES:
            return version, len(versions) == 1
    raise Problems(f"No {platform} version is being prepared in App Store Connect; add one (+ Version) first.")


def localization(api, version_id, locale):
    for item in api.get_all(f"/v1/appStoreVersions/{version_id}/appStoreVersionLocalizations"):
        if item["attributes"]["locale"] == locale:
            return item["id"]
    created = api.request("POST", "/v1/appStoreVersionLocalizations", {"data": {
        "type": "appStoreVersionLocalizations", "attributes": {"locale": locale},
        "relationships": {"appStoreVersion": {"data": {"type": "appStoreVersions", "id": version_id}}}}})
    return created["data"]["id"]


def patch(api, resource_type, resource_id, attributes):
    api.request("PATCH", f"/v1/{resource_type}/{resource_id}",
                {"data": {"type": resource_type, "id": resource_id, "attributes": attributes}})


def upload_metadata(api, version, first_version, locale, texts):
    by_resource = {}
    for name, text in texts.items():
        resource, attribute, _ = FIELDS[name]
        if name == "whats_new.txt" and first_version:
            print(f"  {locale}: skipping whatsNew, which a first version can't have")
        elif resource != "appInfoLocalization":
            by_resource.setdefault(resource, {})[attribute] = text
    if not by_resource:
        return

    if "appStoreVersion" in by_resource:
        patch(api, "appStoreVersions", version["id"], by_resource["appStoreVersion"])
    if "appStoreVersionLocalization" in by_resource:
        patch(api, "appStoreVersionLocalizations", localization(api, version["id"], locale),
              by_resource["appStoreVersionLocalization"])
    print(f"  {locale}: {', '.join(sorted(a for attributes in by_resource.values() for a in attributes))}")


def upload_app_info(api, app_id, locale, texts):
    attributes = {FIELDS[n][1]: t for n, t in texts.items() if FIELDS[n][0] == "appInfoLocalization"}
    if not attributes:
        return
    infos = [i for i in api.get_all(f"/v1/apps/{app_id}/appInfos") if state_of(i) in EDITABLE_STATES]
    if not infos:
        raise Problems("No editable app information in App Store Connect (is a version in review?).")
    for item in api.get_all(f"/v1/appInfos/{infos[0]['id']}/appInfoLocalizations"):
        if item["attributes"]["locale"] == locale:
            patch(api, "appInfoLocalizations", item["id"], attributes)
            print(f"  App information, {locale}: {', '.join(sorted(attributes))}")
            return
    raise Problems(f"The app has no {locale} localization; add the language in App Store Connect first.")


def upload_screenshots(api, localization_id, display_type, files):
    sets = [s for s in api.get_all(f"/v1/appStoreVersionLocalizations/{localization_id}/appScreenshotSets")
            if s["attributes"]["screenshotDisplayType"] == display_type]
    if sets:
        set_id = sets[0]["id"]
        for screenshot in api.get_all(f"/v1/appScreenshotSets/{set_id}/appScreenshots"):
            api.request("DELETE", f"/v1/appScreenshots/{screenshot['id']}")
    else:
        set_id = api.request("POST", "/v1/appScreenshotSets", {"data": {
            "type": "appScreenshotSets", "attributes": {"screenshotDisplayType": display_type},
            "relationships": {"appStoreVersionLocalization": {
                "data": {"type": "appStoreVersionLocalizations", "id": localization_id}}}}})["data"]["id"]

    ids = []
    for file in files:
        content = file.read_bytes()
        reservation = api.request("POST", "/v1/appScreenshots", {"data": {
            "type": "appScreenshots", "attributes": {"fileName": file.name, "fileSize": len(content)},
            "relationships": {"appScreenshotSet": {"data": {"type": "appScreenshotSets", "id": set_id}}}}})["data"]
        for operation in reservation["attributes"]["uploadOperations"]:
            part = content[operation["offset"]:operation["offset"] + operation["length"]]
            request = urllib.request.Request(operation["url"], data=part, method=operation["method"])
            for header in operation.get("requestHeaders") or []:
                request.add_header(header["name"], header["value"])
            with urllib.request.urlopen(request, timeout=300):
                pass
        patch(api, "appScreenshots", reservation["id"],
              {"uploaded": True, "sourceFileChecksum": hashlib.md5(content).hexdigest()})
        ids.append(reservation["id"])

    # Wait for processing, so a rejected image fails this run instead of showing up later.
    pending, deadline = set(ids), time.time() + 600
    while pending and time.time() < deadline:
        time.sleep(5)
        for screenshot_id in list(pending):
            delivery = api.request("GET", f"/v1/appScreenshots/{screenshot_id}")["data"]["attributes"][
                "assetDeliveryState"]
            if delivery["state"] == "COMPLETE":
                pending.discard(screenshot_id)
            elif delivery["state"] == "FAILED":
                raise Problems(f"App Store Connect rejected a {display_type} screenshot: {delivery.get('errors')}")
    if pending:
        raise Problems(f"{len(pending)} {display_type} screenshots were still processing after 10 minutes.")

    api.request("PATCH", f"/v1/appScreenshotSets/{set_id}/relationships/appScreenshots",
                {"data": [{"type": "appScreenshots", "id": i} for i in ids]})


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--screenshots", action="store_true", help="upload the screenshots")
    parser.add_argument("--metadata", action="store_true", help="upload the texts")
    parser.add_argument("--check", action="store_true", help="only validate the files")
    args = parser.parse_args()
    if not (args.screenshots or args.metadata):
        parser.error("pick --screenshots, --metadata or both")

    try:
        screenshots = collect_screenshots() if args.screenshots else {}
        metadata = collect_metadata() if args.metadata else {}
    except Problems as problems:
        for line in str(problems).splitlines():
            error(line)
        return 1

    for (locale, device), files in screenshots.items():
        print(f"{locale} {device} ({DEVICES[device][1]}): {', '.join(f.name for f in files)}")
    for locale, texts in metadata.items():
        print(f"{locale} texts: {', '.join(texts)}")
    if args.check:
        print("The listing files are valid.")
        return 0

    settings = {name: os.environ.get(name, "") for name in
                ("APP_STORE_CONNECT_KEY_ID", "APP_STORE_CONNECT_ISSUER_ID", "APP_STORE_CONNECT_PRIVATE_KEY",
                 "APP_STORE_APP_ID")}
    missing = [name for name, value in settings.items() if not value]
    if missing:
        error(f"Can't upload the listing, missing: {', '.join(missing)}. See docs/releasing.md.")
        return 1

    api = AppStoreConnect(settings["APP_STORE_CONNECT_KEY_ID"], settings["APP_STORE_CONNECT_ISSUER_ID"],
                          settings["APP_STORE_CONNECT_PRIVATE_KEY"])
    app_id = settings["APP_STORE_APP_ID"]
    try:
        for platform in ("IOS", "MAC_OS"):
            devices = [(locale, device) for (locale, device) in screenshots if DEVICES[device][0] == platform]
            if not devices and not metadata:
                continue
            version, first_version = editable_version(api, app_id, platform)
            print(f"{platform} {version['attributes']['versionString']} ({state_of(version)})")
            for locale, texts in metadata.items():
                upload_metadata(api, version, first_version, locale, texts)
            for locale, device in devices:
                display_type = DEVICES[device][1]
                upload_screenshots(api, localization(api, version["id"], locale), display_type,
                                   screenshots[(locale, device)])
                print(f"  {locale} {display_type}: {len(screenshots[(locale, device)])} screenshots")
        for locale, texts in metadata.items():
            upload_app_info(api, app_id, locale, texts)
    except Problems as problem:
        error(str(problem))
        return 1
    finally:
        api.close()
    print("Done. Check the listing in App Store Connect before you submit.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
