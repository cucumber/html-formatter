version:
	$(eval SNAPSHOT_VERSION := $(shell mvn help:evaluate -Dexpression=project.version -q -DforceStdout 2> /dev/null))
	@echo $(subst -SNAPSHOT,,$(SNAPSHOT_VERSION))

version-set:
	@([[ "$(NEXT_VERSION)" ]] || (echo "Please set NEXT_VERSION" && exit 1))
	@mvn versions:set -DnewVersion=$(NEW_VERSION)-SNAPSHOT

release-prepare:
	@mvn release:prepare

.PHONY: version
