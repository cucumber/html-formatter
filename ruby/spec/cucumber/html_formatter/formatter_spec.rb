# frozen_string_literal: true

describe Cucumber::HTMLFormatter::Formatter do
  subject(:formatter) do
    formatter = described_class.new(out)
    allow(formatter).to receive(:assets_loader).and_return(assets)
    formatter
  end

  let(:out) { StringIO.new }
  let(:fake_assets) do
    Class.new do
      def template
        '<html>{{css}}<body>{{messages}}</body>{{script}}</html>'
      end

      def css
        '<style>div { color: red }</style>'
      end

      def script
        "<script>alert('Hi')</script>"
      end
    end
  end
  let(:assets) { fake_assets.new }

  describe '#process_messages' do
    let(:message) { Cucumber::Messages::Envelope.new(pickle: Cucumber::Messages::Pickle.new(id: 'some-random-uid')) }
    let(:expected_report) do
      <<~REPORT.strip
        <html>
        <style>div { color: red }</style>
        <body>
        #{message.to_json}</body>
        <script>alert('Hi')</script>
        </html>
      REPORT
    end

    it 'produces the full html report' do
      formatter.process_messages([message])

      expect(out.string).to eq(expected_report)
    end
  end

  describe '#write_pre_message' do
    it 'outputs the content of the template up to {{messages}}' do
      formatter.write_pre_message

      expect(out.string).to eq("<html>\n<style>div { color: red }</style>\n<body>\n")
    end

    it 'does not write the content twice' do
      formatter.write_pre_message
      formatter.write_pre_message

      expect(out.string).to eq("<html>\n<style>div { color: red }</style>\n<body>\n")
    end
  end

  describe '#write_message' do
    let(:message) { Cucumber::Messages::Envelope.new(pickle: Cucumber::Messages::Pickle.new(id: 'some-random-uid')) }

    it 'appends the message to out' do
      formatter.write_message(message)

      expect(out.string).to eq(message.to_json)
    end

    it 'adds commas between the messages' do
      formatter.write_message(message)
      formatter.write_message(message)

      expect(out.string).to eq("#{message.to_json},\n#{message.to_json}")
    end
  end

  describe '#write_post_message' do
    it 'outputs the template end' do
      formatter.write_post_message

      expect(out.string).to eq("</body>\n<script>alert('Hi')</script>\n</html>")
    end
  end

  context 'when using the CCK' do
    Cucumber::CompatibilityKit.gherkin_examples.each do |example_name|
      def run_formatter(messages)
        out = StringIO.new
        formatter = Cucumber::HTMLFormatter::Formatter.new(out)
        formatter.process_messages(messages)
        out.string
      end

      describe "'#{example_name}' example" do
        subject(:html_report) { run_formatter(File.readlines(example_ndjson)) }

        let(:example_ndjson) { "#{Cucumber::CompatibilityKit.example_path(example_name)}/#{example_name}.feature.ndjson" }

        it { is_expected.to match(/\A<!DOCTYPE html>\s?<html/) }
        it { is_expected.to match(/<\/html>\Z/) }
      end
    end

    Cucumber::CompatibilityKit.markdown_examples.each do |example_name|
      describe "'#{example_name}' example" do
        subject(:html_report) { run_formatter(File.readlines(example_ndjson)) }

        let(:example_ndjson) { "#{Cucumber::CompatibilityKit.example_path(example_name)}/#{example_name}.feature.md.ndjson" }

        it { is_expected.to match(/\A<!DOCTYPE html>\s?<html/) }
        it { is_expected.to match(/<\/html>\Z/) }
      end
    end
  end
end
