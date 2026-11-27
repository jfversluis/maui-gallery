#!/usr/bin/env bash
# Uploads a signed .ipa (iOS) or .pkg (Mac Catalyst) to App Store Connect, where it shows up in TestFlight.
# Uses altool from the selected Xcode with an App Store Connect API key; notarization isn't needed for the App Store.
#
# Usage: eng/release/upload-app-store.sh <file.ipa|file.pkg>
# Environment: APP_STORE_CONNECT_KEY_ID and APP_STORE_CONNECT_ISSUER_ID (repository variables),
# APP_STORE_CONNECT_PRIVATE_KEY (the contents of AuthKey_<key id>.p8, a secret in the app-store environment).
set -euo pipefail

file="${1:?Usage: $0 <file.ipa|file.pkg>}"
if [ ! -f "$file" ]; then
  echo "::error::$file doesn't exist."
  exit 1
fi

missing=()
[ -n "${APP_STORE_CONNECT_KEY_ID:-}" ] || missing+=("APP_STORE_CONNECT_KEY_ID (variable)")
[ -n "${APP_STORE_CONNECT_ISSUER_ID:-}" ] || missing+=("APP_STORE_CONNECT_ISSUER_ID (variable)")
[ -n "${APP_STORE_CONNECT_PRIVATE_KEY:-}" ] || missing+=("APP_STORE_CONNECT_PRIVATE_KEY (secret in the app-store environment)")
if (( ${#missing[@]} > 0 )); then
  echo "::error::Can't upload to App Store Connect, missing: ${missing[*]}. See docs/releasing.md."
  exit 1
fi

key_dir="${RUNNER_TEMP:-$(pwd)}/app-store-connect"
key_file="$key_dir/AuthKey_$APP_STORE_CONNECT_KEY_ID.p8"
mkdir -p "$key_dir"
trap 'rm -rf "$key_dir"' EXIT
(umask 077 && printf '%s\n' "$APP_STORE_CONNECT_PRIVATE_KEY" > "$key_file")

echo "Uploading $(basename "$file") to App Store Connect"
# --wait returns once App Store Connect has processed the build, so processing errors fail this job.
xcrun altool --upload-package "$file" \
  --api-key "$APP_STORE_CONNECT_KEY_ID" \
  --api-issuer "$APP_STORE_CONNECT_ISSUER_ID" \
  --p8-file-path "$key_file" \
  --wait \
  --show-progress
