# frozen_string_literal: true

describe Cucumber::HTMLFormatter::AssetsLoader do
  describe '.template' do
    it 'reads the content of assets/index.mustache.html' do
      expect(File).to receive(:read).with(a_string_ending_with('assets/index.mustache.html'))

      described_class.template
    end
  end

  describe '.css' do
    it 'reads the content of assets/main.css' do
      expect(File).to receive(:read).with(a_string_ending_with('assets/main.css'))

      described_class.css
    end
  end

  describe '.script' do
    it 'reads the content of assets/main.js' do
      expect(File).to receive(:read).with(a_string_ending_with('assets/main.js'))

      described_class.script
    end
  end
end
