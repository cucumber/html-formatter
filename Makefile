javascript_source = $(wildcard javascript/src/*)
assets = main.css main.js main.js.LICENSE.txt index.mustache.html icon.url
ruby_assets = $(addprefix ruby/assets/,$(assets))
java_assets = $(addprefix java/src/main/resources/io/cucumber/htmlformatter/,$(assets))
dotnet_assets = $(addprefix dotnet/Cucumber.HtmlFormatter/Resources/,$(assets))

.DEFAULT_GOAL = help

help: ## Show this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target>\n\nWhere <target> is one of:\n"} /^[$$()% a-zA-Z_-]+:.*?##/ { printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2 } /^##@/ { printf "\n\033[1m%s\033[0m\n", substr($$0, 5) } ' $(MAKEFILE_LIST)

prepare: $(ruby_assets) $(java_assets) $(dotnet_assets) ## Build javascript module and copy required artifacts to java and ruby projects

clean: ## Remove javascript built module and related artifacts from java and ruby projects
	rm -rf $(ruby_assets) $(java_assets) $(dotnet_assets) javascript/dist
.PHONY: .clean

ruby/assets/%: javascript/dist/src/%
	cp $< $@

ruby/assets/%: javascript/dist/%
	cp $< $@

java/src/main/resources/io/cucumber/htmlformatter/%: javascript/dist/src/%
	cp $< $@

java/src/main/resources/io/cucumber/htmlformatter/%: javascript/dist/%
	cp $< $@

dotnet/Cucumber.HtmlFormatter/Resources/%: javascript/dist/src/%
	cp $< $@

dotnet/Cucumber.HtmlFormatter/Resources/%: javascript/dist/%
	cp $< $@

javascript/dist/main.js.LICENSE.txt: javascript/dist/main.js

javascript/dist/main.css: javascript/dist/main.js

javascript/dist/main.js: javascript/package.json $(javascript_source)
	cd javascript && npm install-ci-test && npm run build
	cd ..
	ls javascript/dist
	ls javascript/dist/src
