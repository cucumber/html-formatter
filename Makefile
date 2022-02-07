# make prepare - ready to build ruby and java => build javascript, copy files

JAVASCRIPT_SRC = $(wildcard javascript/src/*)

# 1. npm install[-test]
# 2. npm run build
# 3. cp files

# prepare: javascript/dist/main.js
# cp files

javascript/dist/main.js: javascript/package.json $(JAVASCRIPT_SRC)
	cd javascript && npm install-test && npm run build

# target/classes/io/cucumber/htmlformatter/cucumber-html.js