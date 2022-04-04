javascript_source = $(wildcard javascript/src/*)
assets = cucumber-html.css cucumber-html.js index.mustache.html
ruby_assets = $(addprefix ruby/assets/,$(assets))
java_assets = $(addprefix java/target/classes/io/cucumber/htmlformatter/,$(assets))

.DEFAULT_GOAL = help

help: ## Show this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target>\n\nWhere <target> is one of:\n"} /^[$$()% a-zA-Z_-]+:.*?##/ { printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2 } /^##@/ { printf "\n\033[1m%s\033[0m\n", substr($$0, 5) } ' $(MAKEFILE_LIST)

prepare: $(ruby_assets) $(java_assets) ## Build javascript module and copy required artifacts to java and ruby projects

clean: ## Remove javascript built module and related artifacts from java and ruby projects
	rm -rf $(ruby_assets) $(java_assets) javascript/dist
.PHONY: .clean

ruby/assets/cucumber-html.css: javascript/dist/main.css
	cp $< $@

ruby/assets/cucumber-html.js: javascript/dist/main.js
	cp $< $@

ruby/assets/index.mustache.html: javascript/dist/src/index.mustache.html
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/cucumber-html.css: javascript/dist/main.css java/target/classes/io/cucumber/htmlformatter
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/cucumber-html.js: javascript/dist/main.js java/target/classes/io/cucumber/htmlformatter
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/index.mustache.html: javascript/dist/src/index.mustache.html java/target/classes/io/cucumber/htmlformatter
	cp $< $@

java/target/classes/io/cucumber/htmlformatter:
	mkdir -p $@

javascript/dist/src/index.mustache.html: javascript/dist/main.js

javascript/dist/main.css: javascript/dist/main.js

javascript/dist/main.js: javascript/package.json $(javascript_source)
	cd javascript && npm install-test && npm run build

RELEASED_VERSION = $(shell changelog latest)
JAVA_VERSION = $(shell cd java && make version)
RUBY_VERSION = $(shell cd ruby && make version)
JAVASCRIPT_VERSION = $(shell cd javascript && make version)
CURRENT_BRANCH = $(shell git rev-parse --abbrev-ref HEAD)

language-versions-check:
	@if [[ ($(JAVA_VERSION) != $(RUBY_VERSION)) || ($(JAVA_VERSION) != $(JAVASCRIPT_VERSION)) ]];\
	then \
	echo "Language are inconsistent!"; \
	echo "Java: \t\t$(JAVA_VERSION)"; \
	echo "JavaScript: \t$(JAVASCRIPT_VERSION)"; \
	echo "Ruby: \t\t$(RUBY_VERSION)"; \
	exit 1; \
	fi
.PHONY: language-versions-check

release-version-check:
	@if [[ "$(RELEASED_VERSION)" = "$(JAVA_VERSION)" ]]; \
	then \
		echo "Java version is same as last release version!"; \
		exit 1; \
	fi
	@if [[ "$(RELEASED_VERSION)" = "$(JAVASCRIPT_VERSION)" ]]; \
	then \
		echo "JavaScript version is same as last release version!"; \
		exit 1; \
	fi
	@if [[ "$(RELEASED_VERSION)" = "$(RUBY_VERSION)" ]]; \
	then \
		echo "Ruby version is same as last release version!"; \
		exit 1; \
	fi
.PHONY: release-version-check

version: language-versions-check release-version-check ## Show the next version to be released
	@echo ""
	@echo "The latest released version of html-formatter is $(RELEASED_VERSION)"
	@echo
	@echo "The next version of html-formatter will be $(RUBY_VERSION) and released from '$(CURRENT_BRANCH)'"
	@echo ""
.PHONY: version

version-set: ## Set the next version to be released (requires NEXT_VERSION environment variable)
	@([[ "$(NEXT_VERSION)" ]] || (echo "Please set NEXT_VERSION" && exit 1))
	@cd java && make version-set
	@cd javascript && make version-set
	@cd ruby && make version-set

release:
	commit=$(shell git rev-parse head)
	NEXT_VERSION=$(JAVA_VERSION)
	@cd java && make release-prepare
	@cd javascript && make release-prepare
	@cd ruby && make release-prepare
	git push origin $(commit):refs/heads/release/v$(NEXT_VERSION)