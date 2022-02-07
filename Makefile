JAVASCRIPT_SRC = $(wildcard javascript/src/*)
ASSETS = cucumber-html.css cucumber-html.js index.mustache.html
RUBY_ASSETS = $(addprefix ruby/assets/,$(ASSETS))
JAVA_ASSETS = $(addprefix java/target/classes/io/cucumber/htmlformatter/,$(ASSETS))

prepare: $(RUBY_ASSETS) $(JAVA_ASSETS)

ruby/assets/cucumber-html.css: javascript/dist/main.css
	cp $< $@

ruby/assets/cucumber-html.js: javascript/dist/main.js
	cp $< $@

ruby/assets/index.mustache.html: javascript/dist/src/index.mustache.html 
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/cucumber-html.css: javascript/dist/main.css
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/cucumber-html.js: javascript/dist/main.js
	cp $< $@

java/target/classes/io/cucumber/htmlformatter/index.mustache.html: javascript/dist/src/index.mustache.html 
	cp $< $@

javascript/dist/src/index.mustache.html: javascript/dist/main.js

javascript/dist/main.css: javascript/dist/main.js

javascript/dist/main.js: javascript/package.json $(JAVASCRIPT_SRC)
	cd javascript && npm install-test && npm run build

clean:
	rm -rf $(RUBY_ASSETS) $(JAVA_ASSETS) javascript/dist