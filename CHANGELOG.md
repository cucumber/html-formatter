# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]
### Fixed
- Use correct favicon ([#341](https://github.com/cucumber/html-formatter/pull/341))

### Changed
- [JavaScript] Default to built-in CSS and JS files ([#344](https://github.com/cucumber/html-formatter/pull/344))

## [21.7.0] - 2024-08-12
### Changed
- Updated dependencies to support latest messages
- [Ruby] Reduce object creation when creating/writing to templates ([#322](https://github.com/cucumber/html-formatter/pull/322))

## [21.6.0] - 2024-08-05
### Added
- Add named export for `CucumberHtmlStream` ([#320](https://github.com/cucumber/html-formatter/pull/320))

## [21.5.0] - 2024-08-02
### Changed
- Upgrade `react-components` to [22.3.0](https://github.com/cucumber/react-components/releases/tag/v22.3.0)

## [21.4.1] - 2024-07-18
### Fixed
- Escape json when writing in html ([#312](https://github.com/cucumber/html-formatter/pull/312))

## [21.4.0] - 2024-06-21
### Changed
- Upgrade `react-components` to [22.2.0](https://github.com/cucumber/react-components/releases/tag/v22.2.0)

## [21.3.1] - 2024-03-26
### Fixed
- Correct repo URL in `package.json`

## [21.3.0] - 2024-03-15
### Changed
- Upgrade `react-components` to [22.1.0](https://github.com/cucumber/react-components/releases/tag/v22.1.0) ([#275](https://github.com/cucumber/html-formatter/pull/293))

## [21.2.0] - 2023-12-21
### Changed
- Upgrade `react-components` to [22.0.0](https://github.com/cucumber/react-components/releases/tag/v22.0.0) ([#275](https://github.com/cucumber/html-formatter/pull/275))

### Fixed
- [Ruby] Fixed up All remaining rubocop offenses / removed excess crud ([#276](https://github.com/cucumber/html-formatter/pull/276))

## [21.1.0] - 2023-12-12
### Changed
- Upgrade `messages` to 24.0.0

### Fixed
- [Ruby] Fixed up 90% of all rubocop offenses ([#270](https://github.com/cucumber/html-formatter/pull/270))
- [Java] Fix project urls in `pom.xml`

## [21.0.0] - 2023-11-14
### Added
- [Ruby] CI: Test on ruby 3.1/3.2 ([#268](https://github.com/cucumber/html-formatter/pull/268))

### Changed
- [Ruby] Minimum ruby version is now 2.6+ ([#268](https://github.com/cucumber/html-formatter/pull/268))
- [Ruby] Add placeholder rubocop files ready for rubocop refactoring ([#268](https://github.com/cucumber/html-formatter/pull/268))

## [20.4.0] - 2023-07-13
### Changed
- Upgrade `@cucumber/react-components` to 21.1.1

## [20.3.1] - 2023-06-02
### Fixed
- Set viewport width to device width

## [20.3.0] - 2023-04-07
### Changed
- Upgrade `messages` to 22.0.0

## [20.2.1] - 2022-12-17
### Fixed
- [Java] Allow `io.cucumber:messages:18.0.0` as the minimum version

## [20.2.0] - 2022-11-27
### Changed
- Upgrade `@cucumber/react-components` to `21.0.1`
- Upgrade to React 18
- [Java] Enabled reproducible builds

## [20.1.0] - 2022-09-14
### Changed
- Upgrade `@cucumber/react-components` to `^20.2.0`

## [20.0.0] - 2022-08-07
### Changed
- A variety of dependabot updates

## [19.2.0] - 2022-05-27
### Changed
- Upgrade to `@cucumber/react-components` `^20.1.0`

## [19.1.0] - 2022-04-15
### Changed
- A variety of dependabot updates

## [19.0.0] - 2022-03-25
### Changed
- Upgrade `@cucumber/react` to `^19.2.0`

### Removed
- Binary command that accepted ndjson on stdin to create a report ([#4](https://github.com/cucumber/html-formatter/issues/4))

## [18.0.0] - 2022-01-26
### Added
- Use `auto` theme to automatically render with light or dark theme per platform preference.

### Changed
- Upgrade `@cucumber/react` to `^19.0.0`

## [17.0.0] - 2021-09-02
### Changed
- Upgrade `@cucumber/react` to `^18.0.0`

### Fixed
- [Java] Fix a regression from 14.0.0 where the embedded JavaScript had the wrong content,
preventing reports from displaying properly.

## [16.0.1] - 2021-07-19
### Changed
- Upgrade `messages` to 17.0.1

## [16.0.0] - 2021-07-08
### Added
- Add filters and highlighted search terms in URL query parameter of HTML reports to make sharing those easier
([#1302](https://github.com/cucumber/cucumber/pull/1302))

### Changed
- Upgrade Cucumber Messages to 17.0.0

## [15.0.2] - 2021-05-27
### Fixed
- Upgrade to `@cucumber/react` `16.1.0`

## [15.0.1] - 2021-05-27
### Fixed
- Upgrade to `@cucumber/react` `16.0.2`, fixing a couple of bugs

## [15.0.0] - 2021-05-26
### Changed
- Upgrade to `@cucumber/react` `16.0.0`

## [14.0.0] - 2021-05-17
### Changed
- Upgrade to messages 16.0.0

## [13.0.0] - 2021-04-06
### Added
- Add inline logo icon to page

### Changed
- Upgrade dependencies including `@cucumber/gherkin` ^18.0.0,
`@cucumber/messages` ^15.0.0, `@cucumber/query` ^9.0.2 and
`@cucumber/react` ^13.0.0

## [12.0.0] - 2021-02-08
### Changed
- Upgrade `gherkin` to 17.0.0
- Upgrade `messages` to 14.0.0
- Upgrade `query` to 8.0.0

### Fixed
- Reclassified bundled dependencies as `devDependencies` only ([#1308](https://github.com/cucumber/cucumber/pull/1308))

## [11.0.4] - 2020-12-18
### Fixed
- Fix Java release

## [11.0.3] - 2020-12-18
### Fixed
- Downgrade to Webpack 4.44.2 since the Webpack 5 build fails to load in browsers.

## [11.0.2] - 2020-12-17
### Fixed
- Upgrade `@cucumber/react`

## [11.0.1] - 2020-12-17
### Fixed
- Upgrade `@cucumber/react`

## [11.0.0] - 2020-12-17
### Fixed
- Nothing changed, just tagged a new release to trigger build

## [10.0.0] - 2020-11-04
### Changed
- Upgrade `@cucumber/react`

### Fixed
- [JavaScript] Fix CSS resolution when installed locally [#1180](https://github.com/cucumber/cucumber/pull/1180)

## [9.0.0] - 2020-08-08
### Changed
- Update `messages` to 13.0.1
- Update `messages` to 10.0.0

## [8.0.0] - 2020-08-07
### Changed
- Updated `react` to v9.0.0

## [7.2.0] - 2020-07-31
### Changed
- Updated `react` to v8.1.0
- Updated `messages` to v12.4.0

## [7.1.0] - 2020-07-30
### Changed
- Use `FilteredResults` as the entry point for the reporter ([#1111](https://github.com/cucumber/cucumber/pull/1111))
- Use `react` 8.1.0

## [7.0.0] - 2020-06-29
### Changed
- Upgrade `@cucumber/react` and other dependencies

## [6.0.3] - 2020-06-12
### Fixed
- [JavaScript] Fixed a bug where the command-line interface would always exit with 1
even if there were no errors.
- [Java] Always use UTF-8 encoding

## [6.0.2] - 2020-05-01
### Added
- [Java] Enable consumers to find our version at runtime using `clazz.getPackage().getImplementationVersion()` by upgrading to `cucumber-parent:2.1.0`
([#976](https://github.com/cucumber/cucumber/pull/976) [aslakhellesoy](https://github.com/aslakhellesoy))

### Fixed
- [Java] Use version range for messages dependency
([#986](https://github.com/cucumber/cucumber/pull/986) [mpkorstanje](https://github.com/mpkorstanje))
- [Java] Make writer idempotent when failing to close underlying writer
([#986](https://github.com/cucumber/cucumber/pull/986) [mpkorstanje](https://github.com/mpkorstanje))

## [6.0.1] - 2020-04-15
### Fixed
- Fix Ruby release ([#970](https://github.com/cucumber/cucumber/pull/970) [aslakhellesoy](https://github.com/aslakhellesoy))

## [6.0.0] - 2020-04-14
### Changed
- Upgrade to messages 12.0.0
- Upgrade to gherkin 13.0.0
- Upgrade to @cucumber/react 7.0.0

## [5.0.0] - 2020-04-01
### Changed
- Upgrade `@cucumber/*` dependencies to next major version

### Fixed
- Fix deprecation warning about `<Wrapper>` (Use `<QueriesWrapper>` instead)

## [4.3.0] - 2020-03-13
### Added
- Ruby implementation ([#931](https://github.com/cucumber/cucumber/pull/931) [vincent-psarga](https://github.com/vincent-psarga))

## [4.2.0] - 2020-03-10
### Added
- Java: New Java implementation
([#922](https://github.com/cucumber/cucumber/pull/922) [mpkorstanje](https://github.com/mpkorstanje))
- JavaScript: Add a mustache template in the JavaScript npm module that other implementations can use
- JavaScript: Add `CucumberHtmlStream` (default export), allowing this module to be used as a library (in Cucumber.js)

### Changed
- It's not _really_ needed. This does break SEO, but that's not a goal for Cucumber HTML reports.
- Using both server side rendering and client side rendering results in conflicting versions of the react dom being used during development.
([#923](https://github.com/cucumber/cucumber/pull/923) [aslakhellesoy](https://github.com/mpkorstanje))
- JavaScript: No server side rendering
- JavaScript Use a custom mustache template engine that streams output. Ported from Java

### Fixed
- [JavaScript] Lower memory footprint - messages are no longer buffered during HTML generation
([#928](https://github.com/cucumber/cucumber/pull/928) [aslakhellesoy](https://github.com/aslakhellesoy))

## [4.1.0] - 2020-03-02
### Added
- Embed CSS in generated HTML ([#911](https://github.com/cucumber/cucumber/pull/911) [aslakhellesoy](https://github.com/vincent-psarga))

## [4.0.0] - 2020-02-15
### Changed
- Upgrade `@cucumber/react` to `4.0.0`

## [3.2.3] - 2020-01-22
### Changed
- Upgrade `@cucumber/react` to `3.3.0`

## [3.2.2] - 2020-01-15
### Fixed
- Nothing changed, just tagged a new release to trigger docker build

## [3.2.1] - 2020-01-14
### Fixed
- Nothing changed, just tagged a new release to trigger docker build

## [3.2.0] - 2020-01-10
### Changed
- [JavaScript] changed module name to `@cucumber/html-formatter`

## [3.1.0] - 2019-12-10
### Changed
- Use cucumber-react 3.1.0

## [3.0.0] - 2019-11-15
### Changed
- Use cucumber-react 3.0.0

## [2.0.3] - 2019-10-21
### Fixed
- Fixed automated build on Docker

## [2.0.2] - 2019-10-21
### Fixed
- Add source map support for better stack traces

## [2.0.1] - 2019-10-18
### Changed
- Upgrade cucumber-react to 2.0.1
- Upgrade cucumber-messages to 6.0.2

## [2.0.0] - 2019-10-10
### Changed
- Upgrade cucumber-messages to 6.0.1

## [1.1.0] - 2019-08-29
### Changed
- Upgraded to cucumber-react 1.1.0

## [1.0.2] - 2019-08-23
### Fixed
- Fixed packaging (again)

## [1.0.1] - 2019-08-23
### Fixed
- Fixed packaging

## [1.0.0] - 2019-08-23
### Added
- First release

[Unreleased]: https://github.com/cucumber/html-formatter/compare/v21.7.0...HEAD
[21.7.0]: https://github.com/cucumber/html-formatter/compare/v21.6.0...v21.7.0
[21.6.0]: https://github.com/cucumber/html-formatter/compare/v21.5.0...v21.6.0
[21.5.0]: https://github.com/cucumber/html-formatter/compare/v21.4.1...v21.5.0
[21.4.1]: https://github.com/cucumber/html-formatter/compare/v21.4.0...v21.4.1
[21.4.0]: https://github.com/cucumber/html-formatter/compare/v21.3.1...v21.4.0
[21.3.1]: https://github.com/cucumber/html-formatter/compare/v21.3.0...v21.3.1
[21.3.0]: https://github.com/cucumber/html-formatter/compare/v21.2.0...v21.3.0
[21.2.0]: https://github.com/cucumber/html-formatter/compare/v21.1.0...v21.2.0
[21.1.0]: https://github.com/cucumber/html-formatter/compare/v21.0.0...v21.1.0
[21.0.0]: https://github.com/cucumber/html-formatter/compare/v20.4.0...v21.0.0
[20.4.0]: https://github.com/cucumber/html-formatter/compare/v20.3.1...v20.4.0
[20.3.1]: https://github.com/cucumber/html-formatter/compare/v20.3.0...v20.3.1
[20.3.0]: https://github.com/cucumber/html-formatter/compare/v20.2.1...v20.3.0
[20.2.1]: https://github.com/cucumber/html-formatter/compare/v20.2.0...v20.2.1
[20.2.0]: https://github.com/cucumber/html-formatter/compare/v20.1.0...v20.2.0
[20.1.0]: https://github.com/cucumber/html-formatter/compare/v20.0.0...v20.1.0
[20.0.0]: https://github.com/cucumber/html-formatter/compare/v19.2.0...v20.0.0
[19.2.0]: https://github.com/cucumber/html-formatter/compare/v19.1.0...v19.2.0
[19.1.0]: https://github.com/cucumber/html-formatter/compare/v19.0.0...v19.1.0
[19.0.0]: https://github.com/cucumber/html-formatter/compare/v18.0.0...v19.0.0
[18.0.0]: https://github.com/cucumber/html-formatter/compare/v17.0.0...v18.0.0
[17.0.0]: https://github.com/cucumber/html-formatter/compare/v16.0.1...v17.0.0
[16.0.1]: https://github.com/cucumber/html-formatter/compare/v16.0.0...v16.0.1
[16.0.0]: https://github.com/cucumber/html-formatter/compare/v15.0.2...v16.0.0
[15.0.2]: https://github.com/cucumber/html-formatter/compare/v15.0.1...v15.0.2
[15.0.1]: https://github.com/cucumber/html-formatter/compare/v15.0.0...v15.0.1
[15.0.0]: https://github.com/cucumber/html-formatter/compare/v14.0.0...v15.0.0
[14.0.0]: https://github.com/cucumber/html-formatter/compare/v13.0.0...v14.0.0
[13.0.0]: https://github.com/cucumber/html-formatter/compare/v12.0.0...v13.0.0
[12.0.0]: https://github.com/cucumber/html-formatter/compare/v11.0.4...v12.0.0
[11.0.4]: https://github.com/cucumber/html-formatter/compare/v11.0.3...v11.0.4
[11.0.3]: https://github.com/cucumber/html-formatter/compare/v11.0.2...v11.0.3
[11.0.2]: https://github.com/cucumber/html-formatter/compare/v11.0.1...v11.0.2
[11.0.1]: https://github.com/cucumber/html-formatter/compare/v11.0.0...v11.0.1
[11.0.0]: https://github.com/cucumber/html-formatter/compare/v10.0.0...v11.0.0
[10.0.0]: https://github.com/cucumber/html-formatter/compare/v9.0.0...v10.0.0
[9.0.0]: https://github.com/cucumber/html-formatter/compare/v8.0.0...v9.0.0
[8.0.0]: https://github.com/cucumber/html-formatter/compare/v7.2.0...v8.0.0
[7.2.0]: https://github.com/cucumber/html-formatter/compare/v7.1.0...v7.2.0
[7.1.0]: https://github.com/cucumber/html-formatter/compare/v7.0.0...v7.1.0
[7.0.0]: https://github.com/cucumber/html-formatter/compare/v6.0.3...v7.0.0
[6.0.3]: https://github.com/cucumber/html-formatter/compare/v6.0.2...v6.0.3
[6.0.2]: https://github.com/cucumber/html-formatter/compare/v6.0.1...v6.0.2
[6.0.1]: https://github.com/cucumber/html-formatter/compare/v6.0.0...v6.0.1
[6.0.0]: https://github.com/cucumber/html-formatter/compare/v5.0.0...v6.0.0
[5.0.0]: https://github.com/cucumber/html-formatter/compare/v4.3.0...v5.0.0
[4.3.0]: https://github.com/cucumber/html-formatter/compare/v4.2.0...v4.3.0
[4.2.0]: https://github.com/cucumber/html-formatter/compare/v4.1.0...v4.2.0
[4.1.0]: https://github.com/cucumber/html-formatter/compare/v4.0.0...v4.1.0
[4.0.0]: https://github.com/cucumber/html-formatter/compare/v3.2.3...v4.0.0
[3.2.3]: https://github.com/cucumber/html-formatter/compare/v3.2.2...v3.2.3
[3.2.2]: https://github.com/cucumber/html-formatter/compare/v3.2.1...v3.2.2
[3.2.1]: https://github.com/cucumber/html-formatter/compare/v3.2.0...v3.2.1
[3.2.0]: https://github.com/cucumber/html-formatter/compare/v3.1.0...v3.2.0
[3.1.0]: https://github.com/cucumber/html-formatter/compare/v3.0.0...v3.1.0
[3.0.0]: https://github.com/cucumber/html-formatter/compare/v2.0.3...v3.0.0
[2.0.3]: https://github.com/cucumber/html-formatter/compare/v2.0.2...v2.0.3
[2.0.2]: https://github.com/cucumber/html-formatter/compare/v2.0.1...v2.0.2
[2.0.1]: https://github.com/cucumber/html-formatter/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/cucumber/html-formatter/compare/v1.1.0...v2.0.0
[1.1.0]: https://github.com/cucumber/html-formatter/compare/v1.0.2...v1.1.0
[1.0.2]: https://github.com/cucumber/html-formatter/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/cucumber/html-formatter/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/cucumber/cucumber/releases/tag/v1.0.0
