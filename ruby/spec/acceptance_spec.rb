require 'cucumber-compatibility-kit'

describe 'bin/cucumber-html-formatter' do
  let(:html_formatter_bin) { 'bin/cucumber-html-formatter' }
  let(:html_formatter_command) { "bundle exec #{html_formatter_bin}" }

  Cucumber::CompatibilityKit.gherkin_examples.each do |example_name|
    describe "'#{example_name}' example" do
      subject(:html_report) { `cat #{example_ndjson} | #{html_formatter_command}` }

      let(:example_ndjson) { "#{Cucumber::CompatibilityKit.example_path(example_name)}/#{example_name}.feature.ndjson" }

      it { is_expected.to match(/\A<!DOCTYPE html>\s?<html/) }
      it { is_expected.to match(/<\/html>\Z/) }
    end
  end

  Cucumber::CompatibilityKit.markdown_examples.each do |example_name|
    describe "'#{example_name}' example" do
      let(:example_ndjson) { "#{Cucumber::CompatibilityKit.example_path(example_name)}/#{example_name}.feature.md.ndjson" }
      subject(:html_report) { `cat #{example_ndjson} | #{html_formatter_command}` }

      it { is_expected.to match(/\A<!DOCTYPE html>\s?<html/) }
      it { is_expected.to match(/<\/html>\Z/) }
    end
  end
end
