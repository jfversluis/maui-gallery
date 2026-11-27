#!/bin/bash
# Release smoke test on an Android emulator (android-release-smoke.yml): cold start times, then a tour of representative
# pages by deep link, with a screenshot each and a crash check. Usage: android-release-smoke.sh <apk> <label> <out-dir> [runs]
# SMOKE_STARTUP_ONLY=1 stops after the start times.
set -u
APK=$1; LABEL=$2; OUT=$3; RUNS=${4:-8}
PKG=com.jfversluis.mauigallery
mkdir -p "$OUT/$LABEL"
adb install -r "$APK" > /dev/null || { echo "install failed"; exit 1; }
ACT=$(adb shell cmd package resolve-activity --brief $PKG | tail -1 | tr -d '\r')
echo "Activity: $ACT"
adb logcat -c
# Warm-up launch (first launch after install does extra work), not counted.
adb shell am start -W -n "$ACT" > /dev/null; sleep 8; adb shell am force-stop $PKG; sleep 2
: > "$OUT/$LABEL/startup.txt"
for i in $(seq 1 $RUNS); do
  adb shell am force-stop $PKG; sleep 3
  t=$(adb shell am start -W -n "$ACT" | grep TotalTime | awk '{print $2}' | tr -d '\r')
  echo "$t" >> "$OUT/$LABEL/startup.txt"
  sleep 4
done
python3 - "$OUT/$LABEL/startup.txt" "$LABEL" <<'PY' | tee -a "$OUT/summary.md"
import statistics, sys
v=[int(x) for x in open(sys.argv[1]).read().split()]
print(f"- **{sys.argv[2]}** cold start TotalTime (ms): median {statistics.median(v):.0f}, min {min(v)}, max {max(v)}, runs {v}")
PY
[ "${SMOKE_STARTUP_ONLY:-}" = "1" ] && exit 0

tap_text() { # tap the first node whose text or content-desc matches
  adb shell uiautomator dump /sdcard/ui.xml > /dev/null 2>&1; adb pull /sdcard/ui.xml "$OUT/ui.xml" > /dev/null 2>&1
  python3 - "$OUT/ui.xml" "$1" <<'PY'
import re, sys, xml.etree.ElementTree as ET
want=sys.argv[2]
for n in ET.parse(sys.argv[1]).iter('node'):
    if n.get('text') == want or n.get('content-desc') == want or n.get('resource-id','').endswith(want):
        x1,y1,x2,y2=map(int,re.findall(r'\d+',n.get('bounds')))
        print((x1+x2)//2,(y1+y2)//2); break
PY
}
alive() { adb shell pidof $PKG > /dev/null; }
crashes=0
page() { # page <item-id>
  adb shell am start -W -a android.intent.action.VIEW -d "mauigallery://item/$1" $PKG > /dev/null
  sleep ${2:-6}
  adb exec-out screencap -p > "$OUT/$LABEL/$1.png"
  if ! alive; then echo "- :x: $1: app not running" | tee -a "$OUT/summary.md"; crashes=$((crashes+1)); adb shell am start -n "$ACT" > /dev/null; sleep 6; else echo "- $1: ok" >> "$OUT/summary.md"; fi
}
adb shell am start -W -n "$ACT" > /dev/null; sleep 6
adb exec-out screencap -p > "$OUT/$LABEL/home.png"
for p in button collection-view carousel-view graphics-view data-binding relative-bindings content-view hybrid-web-view map sensors geolocation; do page $p; done
page blazor-web-view 12
# Scroll to the Razor component and click its button: proves Blazor renders and handles events.
read -r SW SH <<< "$(adb shell wm size | tail -1 | tr -d '\r' | sed -E 's/.*: ([0-9]+)x([0-9]+).*/\1 \2/')"
found=""
for i in 1 2 3 4 5 6; do
  xy=$(tap_text "Clicked 0 times"); [ -n "$xy" ] && { found=1; break; }
  adb shell input swipe $((SW/2)) $((SH*3/4)) $((SW/2)) $((SH/3)) 400; sleep 2
done
if [ -n "$found" ]; then adb shell input tap $xy; sleep 2; c=$(tap_text "Clicked 1 time"); echo "- blazor counter click: $([ -n "$c" ] && echo ok || echo ':x: no change')" | tee -a "$OUT/summary.md"; else echo "- :x: blazor counter: Razor button not found" | tee -a "$OUT/summary.md"; fi
adb exec-out screencap -p > "$OUT/$LABEL/blazor-after-click.png"
# Theme switch in Settings (a tab, not a catalog item).
xy=$(tap_text "Settings"); [ -n "$xy" ] && adb shell input tap $xy; sleep 3
xy=$(tap_text "Dark"); [ -n "$xy" ] && adb shell input tap $xy; sleep 3
adb exec-out screencap -p > "$OUT/$LABEL/settings-dark.png"
page button
mv "$OUT/$LABEL/button.png" "$OUT/$LABEL/button-dark.png"
xy=$(tap_text "Settings"); [ -n "$xy" ] && adb shell input tap $xy; sleep 3
xy=$(tap_text "Light"); [ -n "$xy" ] && adb shell input tap $xy; sleep 2
adb logcat -d > "$OUT/$LABEL/logcat.txt"
grep -E "FATAL EXCEPTION| E AndroidRuntime|ClassNotFoundException|NoSuchMethodError|NoClassDefFoundError|Unhandled|MissingMethodException|TypeLoadException" "$OUT/$LABEL/logcat.txt" | head -40 > "$OUT/$LABEL/errors.txt"
echo "- crashes: $crashes, error lines in logcat: $(wc -l < "$OUT/$LABEL/errors.txt")" | tee -a "$OUT/summary.md"
[ $crashes -eq 0 ]
