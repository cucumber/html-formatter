version:
	@cat VERSION

version-set:
	@echo $(NEW_VERSION) > VERSION

release-prepare: version-set
	@git add VERSION
	@git commit -m "Prepare to release v$(NEW_VERSION)"
	@echo "TODO: figure out how to bump ruby version"
	@exit 1

.PHONY: version