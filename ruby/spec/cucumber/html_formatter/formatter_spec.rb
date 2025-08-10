# frozen_string_literal: true

describe Cucumber::HTMLFormatter::Formatter do
  subject(:formatter) { described_class.new(out) }

  let(:out) { StringIO.new }

  context 'when using a simple set of assets' do
    before do
      allow(Cucumber::HTMLFormatter::AssetsLoader)
        .to receive_messages(
          template: "{{title}}{{icon}}{{css}}{{custom_css}}{{messages}}{{script}}{{custom_script}}",
          icon: 'https://example.org/icon.svg',
          css: 'div { color: red }',
          script: "alert('Hi');"
        )
    end

    describe '#process_messages' do
      let(:message) { Cucumber::Messages::Envelope.new(pickle: Cucumber::Messages::Pickle.new(id: 'some-random-uid')) }
      let(:expected_report) do
        <<~REPORT 
          
          Cucumber

          https://example.org/icon.svg

          div { color: red }


          #{message.to_json}
          alert('Hi');


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

        expect(out.string).to eq("\nCucumber\n\nhttps://example.org/icon.svg\n\ndiv { color: red }\n\n\n")
      end

      it 'does not write the content twice' do
        formatter.write_pre_message
        formatter.write_pre_message

        expect(out.string).to eq("\nCucumber\n\nhttps://example.org/icon.svg\n\ndiv { color: red }\n\n\n")
      end
    end

    describe '#write_message' do
      let(:message) { Cucumber::Messages::Envelope.new(pickle: Cucumber::Messages::Pickle.new(id: 'some-random-uid')) }
      let(:message_with_slashes) do
        Cucumber::Messages::Envelope.new(
          gherkin_document: Cucumber::Messages::GherkinDocument.new(
            comments: [Cucumber::Messages::Comment.new(
              location: Cucumber::Messages::Location.new(
                line: 0,
                column: 0
              ),
              text: '</script><script>alert(\'Hello\')</script>'
            )]
          )
        )
      end

      it 'appends the message to out' do
        formatter.write_message(message)

        expect(out.string).to eq(message.to_json)
      end

      it 'adds commas between the messages' do
        formatter.write_message(message)
        formatter.write_message(message)

        expect(out.string).to eq("#{message.to_json},\n#{message.to_json}")
      end

      it 'escapes forward slashes' do
        formatter.write_message(message_with_slashes)

        expect(out.string).to eq('{"gherkinDocument":{"comments":[{"location":{"line":0,"column":0},"text":"<\/script><script>alert(\'Hello\')<\/script>"}]}}')
      end
    end

    describe '#write_post_message' do
      it 'outputs the template end' do
        formatter.write_post_message

        expect(out.string).to eq("\nalert('Hi');\n\n\n")
      end
    end
  end

  context 'when using the CCK' do
    CCK::Examples.gherkin.each do |example_name|
      def run_formatter(messages)
        out = StringIO.new
        formatter = Cucumber::HTMLFormatter::Formatter.new(out)
        formatter.process_messages(messages)
        out.string
      end

      describe "'#{example_name}' example" do
        subject(:html_report) { run_formatter(File.readlines(example_ndjson)) }

        let(:example_ndjson) { "#{CCK::Examples.feature_code_for(example_name)}/#{example_name}.feature.ndjson" }

        it { is_expected.to match(/\A<!DOCTYPE html>\s?<html/) }
        it { is_expected.to match(/<\/html>\Z/) }
      end
    end

    CCK::Examples.markdown.each do |example_name|
      describe "'#{example_name}' example" do
        subject(:html_report) { run_formatter(File.readlines(example_ndjson)) }

        let(:example_ndjson) { "#{CCK::Examples.feature_code_for(example_name)}/#{example_name}.feature.md.ndjson" }

        it { is_expected.to match(/\A<!DOCTYPE html>\s?<html/) }
        it { is_expected.to match(/<\/html>\Z/) }
      end
    end
  end
end
