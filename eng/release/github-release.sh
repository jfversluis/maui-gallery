#!/usr/bin/env bash
# Collects the store packages from the Release workflow's artifacts and attaches them to a GitHub release with
# generated notes. On a v* tag it creates the release (or updates its files when the run is retried); otherwise it
# only lists what it would attach.
#
# Usage: eng/release/github-release.sh <artifacts folder>
# Environment: VERSION and BUILD (from the version job), *_SIGNED (true or false, from the plan job), GH_TOKEN,
# GITHUB_REPOSITORY, GITHUB_REF_TYPE, GITHUB_REF_NAME, GITHUB_SERVER_URL, GITHUB_RUN_ID.
set -euo pipefail
shopt -s globstar nullglob

artifacts="${1:?Usage: $0 <artifacts folder>}"
assets="${RUNNER_TEMP:-$(pwd)}/release-assets"
rm -rf "$assets"
mkdir -p "$assets"

# add <glob> <platform> <signed>
add() {
  # The pattern is expanded on purpose (globstar), so it isn't quoted.
  # shellcheck disable=SC2206
  local matches=($1) platform="$2" signed="$3"
  if (( ${#matches[@]} != 1 )); then
    echo "::error::Expected one $platform package for '$1', found ${#matches[@]}."
    exit 1
  fi
  local file="${matches[0]}" name="MauiGallery-$VERSION-$BUILD-$platform"
  [[ "$signed" == "true" ]] || name="$name-unsigned"
  cp "$file" "$assets/$name.${file##*.}"
  echo "$(basename "$file") -> $name.${file##*.}"
}

add "$artifacts/android-*/**/*-Signed.aab" android "$ANDROID_SIGNED"
add "$artifacts/ios-*/**/*.ipa" ios "$IOS_SIGNED"
add "$artifacts/maccatalyst-*/**/*.pkg" maccatalyst "$MAC_SIGNED"
# The Store signs MSIX packages, so a bundle with the Partner Center identity is what the Store takes.
add "$artifacts/windows-*-msixbundle/**/*.msixbundle" windows "$WINDOWS_IDENTITY"

run_url="$GITHUB_SERVER_URL/$GITHUB_REPOSITORY/actions/runs/$GITHUB_RUN_ID"
notes="Version $VERSION, build $BUILD: the store packages built by [this Release run]($run_url). Get the app from the \
App Store, Mac App Store, Google Play or Microsoft Store once each store has approved it. Packages named *-unsigned \
weren't signed for a store."

if [[ "$GITHUB_REF_TYPE" != "tag" ]]; then
  echo "Not a tag ($GITHUB_REF_NAME): skipping the GitHub release. It would attach:"
  ls -l "$assets"
  exit 0
fi

tag="$GITHUB_REF_NAME"
if gh release view "$tag" > /dev/null 2>&1; then
  echo "Release $tag exists: replacing its files."
  gh release upload "$tag" "$assets"/* --clobber
else
  gh release create "$tag" "$assets"/* --verify-tag --title "$tag" --notes "$notes" --generate-notes
fi
echo "### GitHub release [$tag]($GITHUB_SERVER_URL/$GITHUB_REPOSITORY/releases/tag/$tag)" >> "${GITHUB_STEP_SUMMARY:-/dev/null}"
