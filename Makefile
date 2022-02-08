javascript_source = $(wildcard javascript/src/*)
assets = cucumber-html.css cucumber-html.js index.mustache.html
ruby_assets = $(addprefix ruby/assets/,$(assets))
java_assets = $(addprefix java/target/classes/io/cucumber/htmlformatter/,$(assets))

prepare: $(ruby_assets) $(java_assets)

clean:
	rm -rf $(ruby_assets) $(java_assets) javascript/dist
.PHONY: .clean

ruby/assets/cucumber-html.css: javascript/dist/main.css
	cp $< $@

ruby/assets/cucumber-html.js: javascript/dist/main.js
	cp $< $@

ruby/assets/index.mustache.html: javascript/dist/src/index.mustache.html
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/cucumber-html.css: java/target/classes/io/cucumber/htmlformatter javascript/dist/main.css
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/cucumber-html.js: java/target/classes/io/cucumber/htmlformatter javascript/dist/main.js
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/index.mustache.html: java/target/classes/io/cucumber/htmlformatter javascript/dist/src/index.mustache.html
	cp $< $@

java/target/classes/io/cucumber/htmlformatter:
	mkdir -p $@

javascript/dist/src/index.mustache.html: javascript/dist/main.js

javascript/dist/main.css: javascript/dist/main.js

javascript/dist/main.js: javascript/package.json $(javascript_source)
	cd javascript && npm install-test && npm run build
