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
	cd javascript && npm install-ci-test && npm run build
