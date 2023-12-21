# frozen_string_literal: true

describe Cucumber::HTMLFormatter::TemplateWriter do
  describe '#write_between' do
    subject(:template_writer) { described_class.new(template) }

    let(:template) { 'Some template {{here}} with content after' }

    it 'outputs content of the template between the given words' do
      expect(template_writer.write_between('Some', 'content')).to eq(' template {{here}} with ')
    end

    context 'when the `from` argument is `nil`' do
      it 'outputs template from the beginning' do
        expect(template_writer.write_between(nil, '{{here}}')).to eq('Some template ')
      end
    end

    context 'when the `to` argument is `nil`' do
      it 'outputs content of template after the "from" argument value' do
        expect(template_writer.write_between('{{here}}', nil)).to eq(' with content after')
      end
    end

    context 'when the `from` argument is not contained in the template' do
      it 'renders the template from the beginning' do
        expect(template_writer.write_between('Unknown start', '{{here}}')).to eq('Some template ')
      end
    end
  end
end
