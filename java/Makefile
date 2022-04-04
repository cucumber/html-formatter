SNAPSHOT_VERSION = $(shell mvn help:evaluate -Dexpression=project.version -q -DforceStdout 2> /dev/null)

version:
	@echo $(subst -SNAPSHOT,,$(SNAPSHOT_VERSION))

version-set:
	@mvn versions:set -DnewVersion=$(NEW_VERSION)-SNAPSHOT

release-prepare:
	@mvn release:prepare

.PHONY: version
