version:
	@echo $(shell npm view . version)

version-set:
	@([[ "$(NEXT_VERSION)" ]] || (echo "Please set NEXT_VERSION" && exit 1))
	@npm version $(NEW_VERSION)

release-prepare: version-set
	@git add package*.*
	@git commit -m "Prepare to release v$(NEW_VERSION)"
	@npm version patch
	@git add package*.*
	@git commit -m "prepare for the next development iteration"

.PHONY: version