#!/usr/bin/env bash
# Decides which store uploads the Release workflow runs, and says why the others are skipped.
#
# An upload runs when it's requested (a v* tag requests every store; a manual run picks them with the `stores`
# input), the store is configured (its repository variables exist) and the package is signed (the signing secrets
# exist). Otherwise it's skipped with a notice, so the workflow stays green on forks and before setup. The upload
# credentials are environment secrets that only the approved upload job can read, so that job checks them.
# See docs/releasing.md.
#
# Inputs (environment): EVENT_NAME, STORES, IOS_SIGNED, MAC_SIGNED, ANDROID_SIGNED, WINDOWS_IDENTITY,
# APP_STORE_CONFIGURED, GOOGLE_PLAY_CONFIGURED, MICROSOFT_STORE_CONFIGURED, GOOGLE_PLAY_TRACK, GOOGLE_PLAY_STATUS,
# MICROSOFT_STORE_MODE, MICROSOFT_STORE_FLIGHT_ID, WHATSNEW_DIR.
# Outputs ($GITHUB_OUTPUT): app_store_ios, app_store_mac, google_play and microsoft_store (true or false), and the
# resolved google_play_track, google_play_status and microsoft_store_mode.
set -euo pipefail

output="${GITHUB_OUTPUT:-/dev/stdout}"
summary="${GITHUB_STEP_SUMMARY:-/dev/null}"
docs="docs/releasing.md"
failed=false
# Set by decide: whether each upload runs.
will_google_play=false
will_microsoft_store=false

fail() {
  echo "::error::$1"
  failed=true
}

requested() {
  [[ "$EVENT_NAME" != "workflow_dispatch" || "$STORES" == "all" || "$STORES" == "$1" ]]
}

track="${GOOGLE_PLAY_TRACK:-internal}"
status="${GOOGLE_PLAY_STATUS:-completed}"
mode="${MICROSOFT_STORE_MODE:-draft}"

{
  echo "### Store uploads"
  echo
  echo "| Store | Upload | Notes |"
  echo "| --- | --- | --- |"
} >> "$summary"

# decide <output name> <store> <display name> <configured> <signed> <what configures it> <what signs it> [<details>]
decide() {
  local key="$1" store="$2" name="$3" configured="$4" signed="$5" configure_hint="$6" sign_hint="$7" details="${8:-}"
  local run=false reason
  if ! requested "$store"; then
    reason="Not requested: run the workflow with stores set to all or $store."
  elif [[ "$configured" != "true" ]]; then
    reason="Not configured: set $configure_hint (see $docs)."
  elif [[ "$signed" != "true" ]]; then
    reason="The package isn't signed for the store: set $sign_hint (see $docs)."
  else
    run=true
    reason="Waits for approval in the $store environment.${details:+ $details}"
  fi

  if [[ "$run" == "true" ]]; then
    echo "$name: $reason"
  elif requested "$store"; then
    echo "::notice title=$name upload skipped::$reason"
  fi
  echo "| $name | $([[ "$run" == "true" ]] && echo "Yes" || echo "Skipped") | $reason |" >> "$summary"
  echo "$key=$run" >> "$output"
  printf -v "will_$key" '%s' "$run"
}

apple_config="the APP_STORE_CONNECT_KEY_ID and APP_STORE_CONNECT_ISSUER_ID variables"
decide app_store_ios app-store "App Store Connect: iOS" "$APP_STORE_CONFIGURED" "$IOS_SIGNED" "$apple_config" \
  "the APPLE_CERTIFICATE_P12_BASE64 and APPLE_PROVISIONING_PROFILE_IOS_BASE64 secrets" "Goes to TestFlight."
decide app_store_mac app-store "App Store Connect: Mac" "$APP_STORE_CONFIGURED" "$MAC_SIGNED" "$apple_config" \
  "the APPLE_CERTIFICATE_P12_BASE64, APPLE_INSTALLER_CERTIFICATE_P12_BASE64 and APPLE_PROVISIONING_PROFILE_MAC_BASE64 secrets" \
  "Goes to TestFlight."
decide google_play google-play "Google Play" "$GOOGLE_PLAY_CONFIGURED" "$ANDROID_SIGNED" \
  "the GOOGLE_PLAY_SERVICE_ACCOUNT variable" "the ANDROID_KEYSTORE_BASE64, ANDROID_KEYSTORE_PASSWORD, ANDROID_KEY_ALIAS and ANDROID_KEY_PASSWORD secrets" \
  "Track $track, status $status."
decide microsoft_store microsoft-store "Microsoft Store" "$MICROSOFT_STORE_CONFIGURED" "$WINDOWS_IDENTITY" \
  "the MICROSOFT_STORE_PRODUCT_ID variable" "the WINDOWS_IDENTITY_NAME, WINDOWS_PUBLISHER and WINDOWS_PUBLISHER_DISPLAY_NAME variables" \
  "Mode $mode."

# Check the store options only for the uploads that run, so a build-only run never fails on them.
if [[ "$will_google_play" == "true" ]]; then
  [[ "$track" =~ ^(internal|alpha|beta|production)$ ]] || fail "The Google Play track must be internal, alpha, beta or production, found '$track'."
  [[ "$status" =~ ^(completed|draft)$ ]] || fail "The Google Play release status must be completed or draft, found '$status'."
  # Google Play rejects release notes over 500 characters per language.
  for file in "${WHATSNEW_DIR:-eng/release/whatsnew}"/whatsnew-*; do
    [ -e "$file" ] || continue
    # Command substitution drops the trailing newline, which doesn't count.
    length=$(printf '%s' "$(cat "$file")" | wc -m | tr -d ' ')
    if (( length > 500 )); then
      fail "$file has $length characters; Google Play allows 500."
    fi
  done
fi
if [[ "$will_microsoft_store" == "true" ]]; then
  [[ "$mode" =~ ^(draft|flight|publish)$ ]] || fail "The Microsoft Store mode must be draft, flight or publish, found '$mode'."
  if [[ "$mode" == "flight" && -z "${MICROSOFT_STORE_FLIGHT_ID:-}" ]]; then
    fail "Microsoft Store mode flight needs the MICROSOFT_STORE_FLIGHT_ID variable (Partner Center > Package flights)."
  fi
fi

{
  echo "google_play_track=$track"
  echo "google_play_status=$status"
  echo "microsoft_store_mode=$mode"
} >> "$output"

if [[ "$failed" == "true" ]]; then
  exit 1
fi
