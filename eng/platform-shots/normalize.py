#!/usr/bin/env python3
"""Turns raw platform captures into the images the app ships.

Usage: python3 eng/platform-shots/normalize.py [--raw artifacts/platform-shots/raw] [--platforms ios,android,...]

Reads <raw>/<platform>/<id>-<theme>.png (a DevFlow element screenshot of the sample, at the device's native scale,
or a window screenshot when the .json next to it has a "crop" rectangle) and the .json bounds, then for each image:
trims it to the visible control (transparent captures to their opaque pixels, opaque ones, which are on the stage
color, to the pixels that differ from it), scales it to 2 pixels per
device-independent unit (so every platform shows the control at the same size), adds a uniform 12-unit margin, caps it
at 640 pixels wide and writes an optimized palette PNG to
src/MauiGallery/Resources/Raw/platform-shots/<id>/<platform>-<theme>.png. Needs Pillow (pip install pillow).
"""

import argparse
import io
import json
import pathlib
import sys

from PIL import Image, ImageChops, ImageCms

ROOT = pathlib.Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "src" / "MauiGallery" / "Resources" / "Raw" / "platform-shots"
PLATFORMS = ("ios", "android", "maccatalyst", "windows")
DENSITY = 2  # pixels per device-independent unit in the shipped images
MARGIN = 12  # units of transparent space around the control
MAX_WIDTH = 640


def visible_box(image: Image.Image):
    """The bounding box of the control: non-transparent pixels, or pixels that differ from an opaque background."""
    alpha = image.getchannel("A")
    if alpha.getextrema()[0] < 255:
        return alpha.point(lambda a: 255 if a > 8 else 0).getbbox()
    background = Image.new("RGBA", image.size, image.getpixel((0, 0)))
    difference = ImageChops.difference(image, background).convert("L")
    return difference.point(lambda d: 255 if d > 12 else 0).getbbox()


def to_srgb(image: Image.Image) -> Image.Image:
    """Converts captures tagged with a color profile (iOS and Mac use Display P3) to plain sRGB."""
    icc = image.info.get("icc_profile")
    image = image.convert("RGBA")
    if not icc:
        return image
    source = ImageCms.ImageCmsProfile(io.BytesIO(icc))
    return ImageCms.profileToProfile(image, source, ImageCms.createProfile("sRGB"), outputMode="RGBA")


def palette_image(image: Image.Image) -> Image.Image:
    """Reduces to 256 colors, keeping fully transparent pixels exactly transparent (the quantizer blends them with
    faint shadows, which shows as a tinted box behind the control)."""
    alpha = image.getchannel("A").point(lambda a: 0 if a <= 2 else a)
    image.putalpha(alpha)
    quantized = image.quantize(255, method=Image.Quantize.FASTOCTREE)
    palette = quantized.getpalette(rawmode="RGBA")[: 255 * 4]
    quantized.putpalette(palette + [0] * (255 * 4 - len(palette)) + [0, 0, 0, 0], rawmode="RGBA")
    quantized.paste(255, mask=alpha.point(lambda a: 255 if a == 0 else 0))
    return quantized


def normalize(source: pathlib.Path, platform: str) -> pathlib.Path | None:
    stem = source.stem  # <id>-<theme>
    item_id, theme = stem.rsplit("-", 1)
    image = to_srgb(Image.open(source))
    density = DENSITY
    bounds_file = source.with_suffix(".json")
    if bounds_file.exists():
        bounds = json.loads(bounds_file.read_text(encoding="utf-8-sig"))
        if isinstance(bounds, dict) and isinstance(bounds.get("crop"), dict):
            # A window screenshot at one pixel per unit (Windows), with the sample's position in the window.
            crop = bounds["crop"]
            image = image.crop((round(crop["x"]), round(crop["y"]), round(crop["x"] + crop["width"]), round(crop["y"] + crop["height"])))
        if isinstance(bounds, dict) and bounds.get("width"):
            density = image.width / float(bounds["width"])

    box = visible_box(image)
    if box is None:
        print(f"!! {platform}/{stem}: nothing visible", file=sys.stderr)
        return None
    # Opaque captures are on the stage color, which the app also puts behind the tiles: pad with it.
    background = (0, 0, 0, 0) if image.getchannel("A").getextrema()[0] < 255 else image.getpixel((0, 0))
    image = image.crop(box)

    scale = DENSITY / density
    margin = MARGIN * DENSITY
    if (image.width * scale) + 2 * margin > MAX_WIDTH:
        scale = (MAX_WIDTH - 2 * margin) / image.width
    if abs(scale - 1) > 0.01:
        size = (max(1, round(image.width * scale)), max(1, round(image.height * scale)))
        # Resample with premultiplied alpha so edges don't pick up dark fringes.
        image = image.convert("RGBa").resize(size, Image.Resampling.LANCZOS).convert("RGBA")

    canvas = Image.new("RGBA", (image.width + 2 * margin, image.height + 2 * margin), background)
    canvas.paste(image, (margin, margin))

    target = OUTPUT / item_id / f"{platform}-{theme}.png"
    target.parent.mkdir(parents=True, exist_ok=True)
    palette_image(canvas).save(target, optimize=True)
    return target


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--raw", default=str(ROOT / "artifacts" / "platform-shots" / "raw"))
    parser.add_argument("--platforms", default=",".join(PLATFORMS))
    args = parser.parse_args()

    raw = pathlib.Path(args.raw)
    written = 0
    for platform in args.platforms.split(","):
        for source in sorted((raw / platform).glob("*.png")):
            target = normalize(source, platform)
            if target:
                written += 1
                print(f"{target.relative_to(ROOT)}  {target.stat().st_size // 1024} KB")
    total = sum(p.stat().st_size for p in OUTPUT.rglob("*.png")) if OUTPUT.exists() else 0
    print(f"{written} images written; platform-shots total {total / 1024 / 1024:.2f} MB")
    return 0


if __name__ == "__main__":
    sys.exit(main())
