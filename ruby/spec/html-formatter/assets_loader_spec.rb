# frozen_string_literal: true

require 'cucumber/html_formatter/assets_loader'

describe Cucumber::HTMLFormatter::AssetsLoader do
  subject(:assets_loader) { Cucumber::HTMLFormatter::AssetsLoader.new }

  before do
    allow(File).to receive(:read).and_return('whatever content')
  end

  context '.template' do
    it 'reads the content of assets/index.mustache.html' do
      expect(assets_loader.template).to eq('whatever content')
      expect(File).to have_received(:read).with(/.*\/assets\/index\.mustache\.html$/)
    end
  end

  context '.css' do
    it 'reads the content of assets/main.css' do
      expect(assets_loader.css).to eq('whatever content')
      expect(File).to have_received(:read).with(/.*\/assets\/main\.css$/)
    end
  end

  context '.script' do
    it 'reads the content of assets/main.js' do
      expect(assets_loader.script).to eq('whatever content')
      expect(File).to have_received(:read).with(/.*\/assets\/main\.js$/)
    end
  end
end