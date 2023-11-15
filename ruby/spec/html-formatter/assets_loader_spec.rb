# frozen_string_literal: true

describe Cucumber::HTMLFormatter::AssetsLoader do
  subject(:assets_loader) { described_class.new }

  before do
    allow(File).to receive(:read).and_return('whatever content')
  end

  describe '#template' do
    it 'reads the content of assets/index.mustache.html' do
      expect(File).to receive(:read).with(/.*\/assets\/index\.mustache\.html$/)

      assets_loader.template
    end
  end

  describe '#css' do
    it 'reads the content of assets/main.css' do
      expect(File).to receive(:read).with(/.*\/assets\/main\.css$/)

      assets_loader.css
    end
  end

  describe '#script' do
    it 'reads the content of assets/main.js' do
      expect(File).to receive(:read).with(/.*\/assets\/main\.js$/)

      assets_loader.script
    end
  end
end
