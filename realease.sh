#!/bin/bash
set -e

function showUsage() {
  echo "Usage: $0 [OPTIONS] MAJOR.MINOR.PATCH"
  echo "OPTIONS:"
  echo "  --help        shows this help"
  echo "  --no-git-push do not push to git"
}

# TODO:
# Version2:
# Validate input

# Version3:
# Present the user with the current version?
# Show unreleased
# Integrity check (are all the tools installed)

# Version4:
# Bootstrap from single git repo.
# Add tests

# Version5:
# Version bumping
while [[ $# -gt 0 ]]; do
  case $1 in
  --no-git-push)
    NO_GIT_PUSH="true"
    shift # past argument
    ;;
  -h | --help)
    echo "Makes a release to GitHub"
    showUsage
    exit 0
    ;;
  -* | --*)
    echo "Unknown option $1"
    showUsage
    exit 1
    ;;
  *)
    POSITIONAL_ARGS+=("$1") # save positional arg
    shift                   # past argument
    ;;
  esac
done

set -- "${POSITIONAL_ARGS[@]}" # restore positional parameters

if [[ $# -ne 1 ]]; then
  echo "Missing MAJOR.MINOR.PATCH argument"
  showUsage
  exit 1
fi

NEW_VERSION=$1

if [[ ! "$NEW_VERSION" =~ ^[0-9]+.[0-9]+.[0-9]+$ ]]; then
  echo "Invalid MAJOR.MINOR.PATCH argument: $NEW_VERSION"
  showUsage
  exit 1
fi

if [[ ! "$NEW_VERSION" =~ ^[0-9]+.[0-9]+.[0-9]+$ ]]; then
  echo "Invalid MAJOR.MINOR.PATCH argument: $NEW_VERSION"
  showUsage
  exit 1
fi

if [ -n $(git tag --list "v$NEW_VERSION") ]; then
  echo "Version $NEW_VERSION has already been released"
  exit 1
fi

if ! git diff-index --quiet HEAD; then
  echo "Git has uncommitted changes"
  exit 1
fi

###
## release
###
changelog release "$NEW_VERSION" --tag-format "v%s" -o CHANGELOG.md

pushd javascript
  npm version $NEW_VERSION
popd

pushd java
  mvn versions:set -DnewVersion="$NEW_VERSION"
  mvn versions:set-scm-tag -DnewTag="v$NEW_VERSION"
popd

pushd ruby
  echo "$NEW_VERSION" >VERSION
popd

git commit -am "Prepare release v$NEW_VERSION"
git tag "v$NEW_VERSION"
RELEASE_COMMIT=$(git rev-parse HEAD)

if [[ -z $NO_GIT_PUSH ]]; then
  git push
  git push origin $RELEASE_COMMIT:refs/heads/release/v$(NEW_VERSION)
fi

###
## post release
###
pushd java
  NEW_VERSION_TEMPLATE="\${parsedVersion.majorVersion}.\${parsedVersion.minorVersion}.\${parsedVersion.nextIncrementalVersion}-SNAPSHOT"
  mvn build-helper:parse-version \
    versions:set -DnewVersion="$NEW_VERSION_TEMPLATE" \
    versions:set-scm-tag -DnewTag="HEAD"
popd

git commit -am "Prepare for the next development iteration"
if [[ -z $NO_GIT_PUSH ]]; then
  git push
fi
