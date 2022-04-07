#!/bin/bash
set -e

function release_javascript() {
  if [[ -d javascript ]]; then
    pushd javascript
    npm version $NEW_VERSION
    popd
  fi
}
function post_release_javascript() {
  # noop
  :
}

function release_java() {
  if [[ -d java ]]; then
    pushd java
    mvn --quiet versions:set -DnewVersion="$NEW_VERSION"
    mvn --quiet versions:set-scm-tag -DnewTag="v$NEW_VERSION"
    popd
  fi
}
function post_release_java() {
  pushd java
  NEW_VERSION_TEMPLATE="\${parsedVersion.majorVersion}.\${parsedVersion.minorVersion}.\${parsedVersion.nextIncrementalVersion}-SNAPSHOT"
  mvn --quiet \
    build-helper:parse-version \
    versions:set -DnewVersion="$NEW_VERSION_TEMPLATE" \
    versions:set-scm-tag -DnewTag="HEAD"
  popd
}

function release_ruby() {
  if [[ -d ruby ]]; then
    pushd ruby
    echo "$NEW_VERSION" >VERSION
    popd
  fi
}
function post_release_ruby() {
  # noop
  :
}

# TODO:
# Version3:
# Integrity check (are all the tools installed?)
# Present the user with the current version?
# Show unreleased
# Reduce the output from git and pushd/popo

# Version4:
# Bootstrap from single git repo.
# Add tests

# Version5:
# Version bumping
function showUsage() {
  echo "Usage: $0 [OPTIONS] MAJOR.MINOR.PATCH"
  echo "OPTIONS:"
  echo "  --help           shows this help"
  echo "  --no-git-push    do not push to git"
  echo "  --no-git-commit  do not commit git"
}

while [[ $# -gt 0 ]]; do
  case $1 in
  --no-git-push)
    NO_GIT_PUSH="true"
    shift # past argument
    ;;
  --no-git-commit)
    NO_GIT_COMMIT="true"
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

if [ -n "$(git tag --list "v$NEW_VERSION")" ]; then
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
release_javascript
release_java
release_ruby

changelog release "$NEW_VERSION" --tag-format "v%s" -o CHANGELOG.md

if [[ -z $NO_GIT_COMMIT ]]; then
  git commit -am "Prepare release v$NEW_VERSION"
  git tag "v$NEW_TAG"
fi

###
## post release
###
post_release_javascript
post_release_java
post_release_ruby

if [[ -z $NO_GIT_COMMIT ]]; then
  git commit -am "Prepare for the next development iteration"
fi

###
# push to github
##
if [[ -z $NO_GIT_PUSH ]]; then
  git push origin
  git push origin "$(git rev-list --max-count=1 "v$NEW_VERSION"):refs/heads/release/v$NEW_VERSION"
fi
