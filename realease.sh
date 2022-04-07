#!/bin/bash
set -e

# Version2:
# Present the user with the version

if [[ -z $NEW_VERSION ]]; then
  echo "Please set NEW_VERSION"
  exit 1
fi

if git diff-index --quiet HEAD; then
  echo "Git has uncommitted changes";
  exit 1
fi

## release
changelog release "$NEW_VERSION" --tag-format "v%s" -o CHANGELOG.md

pushd javascript
  npm version $NEW_VERSION
popd

pushd java
  mvn versions:set -DnewVersion="$NEW_VERSION"
  mvn versions:set-scm-tag -DnewTag="v$NEW_VERSION"
popd

pushd ruby
  echo "$NEW_VERSION" > VERSION
popd

git commit -am "Release v$NEW_VERSION"
git tag "v$NEW_VERSION"
RELEASE_COMMIT=$(git rev-parse HEAD)

#git push
#git push origin $RELEASE_COMMIT:refs/heads/release/v$(NEW_VERSION)

## post release

pushd java
  NEW_VERSION_TEMPLATE="\${parsedVersion.majorVersion}.\${parsedVersion.minorVersion}.\${parsedVersion.nextIncrementalVersion}-SNAPSHOT"
  mvn build-helper:parse-version \
    versions:set -DnewVersion="$NEW_VERSION_TEMPLATE" \
    versions:set-scm-tag -DnewTag="HEAD"
popd

git commit -am "Post release v$NEW_VERSION"
#git push
