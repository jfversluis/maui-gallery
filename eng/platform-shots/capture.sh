#!/usr/bin/env bash
# Captures the stage of each page's first example for the "Same code, every platform" tiles.
#
# Usage: eng/platform-shots/capture.sh <ios|android|maccatalyst> --agent-port <port> [--themes light,dark] [--pages id,id]
#
# Run the Debug app with DevFlow on the target first (see AGENTS.md). For each theme, the script switches the app's
# theme in Settings, opens every page listed in src/MauiGallery.Core/Catalog/PlatformShots.cs and saves a DevFlow
# element screenshot of the first example's sample (native resolution; transparent, or on the stage color with the
# dot grid hidden, depending on the platform) to
# artifacts/platform-shots/raw/<platform>/<id>-<theme>.png, with its logical bounds in a .json file next to it.
# Then run normalize.py to trim, pad, scale and optimize them into Resources/Raw/platform-shots. Windows captures
# come from the Windows UI workflow (.github/scripts/windows-ui-crawl.ps1 -Mode PlatformShots).
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
platform="${1:-}"
shift || true
port=""
themes="light,dark"
pages=""
while [[ $# -gt 0 ]]; do
    case "$1" in
        --agent-port) port="$2"; shift 2 ;;
        --themes) themes="$2"; shift 2 ;;
        --pages) pages="$2"; shift 2 ;;
        *) echo "Unknown option $1" >&2; exit 2 ;;
    esac
done
if [[ ! "$platform" =~ ^(ios|android|maccatalyst)$ || -z "$port" ]]; then
    sed -n '2,12p' "$0" >&2
    exit 2
fi

# The sample is the ContentView next to the stage's dot grid; the first match is the page's first example.
selector='DotGridView ~ ContentView'
out="$root/artifacts/platform-shots/raw/$platform"
mkdir -p "$out"

if [[ -z "$pages" ]]; then
    ids=$(sed -nE 's/^ *"([a-z0-9-]+)",$/\1/p' "$root/src/MauiGallery.Core/Catalog/PlatformShots.cs")
else
    ids=$(tr ',' '\n' <<< "$pages")
fi

devflow() { maui devflow "$@" --agent-port "$port"; }

failures=0
for theme in ${themes//,/ }; do
    devflow ui navigate '//main/settings' > /dev/null 2>&1
    sleep 1
    devflow ui tap "theme-$theme" > /dev/null 2>&1
    sleep 1
    for id in $ids; do
        devflow ui navigate "//main/controls/$id" > /dev/null 2>&1
        if ! devflow ui query --selector "$selector" --wait-until exists --timeout 15 > /dev/null 2>&1; then
            echo "!! $id: no stage found" >&2
            failures=$((failures + 1))
            continue
        fi
        # Some platforms crop the element from the window: hide the stage's dot grid so only the control remains.
        for dots in $(devflow ui query --selector DotGridView --fields id --json 2>/dev/null \
            | python3 -c 'import json,sys; print(" ".join(e["id"] for e in json.load(sys.stdin)))'); do
            devflow ui set-property "$dots" IsVisible False > /dev/null 2>&1
        done
        # Let images, web content and entrance animations settle.
        case "$id" in
            image) sleep 5 ;;
            *) sleep 1.5 ;;
        esac
        stage=$(devflow ui query --selector "$selector" --fields id,bounds --json 2>/dev/null \
            | python3 -c 'import json,sys; e=json.load(sys.stdin)[0]; print(e["id"]); print(json.dumps(e["bounds"]), file=sys.stderr)' \
            2> "$out/$id-$theme.json")
        # Android crops element screenshots from the screen, so the stage must be on screen.
        devflow ui scroll --element "$stage" --position Center > /dev/null 2>&1 || true
        sleep 1
        if devflow ui screenshot --id "$stage" --scale native --overwrite --output "$out/$id-$theme.png" > /dev/null 2>&1; then
            echo "$platform $theme $id"
        else
            echo "!! $id: screenshot failed" >&2
            failures=$((failures + 1))
        fi
    done
done

# Leave the app in the system theme.
devflow ui navigate '//main/settings' > /dev/null 2>&1
devflow ui tap theme-system > /dev/null 2>&1 || true
echo "Captured into $out ($failures failures)"
