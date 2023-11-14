version = File.read(File.expand_path("VERSION", __dir__)).strip

Gem::Specification.new do |s|
  s.name        = 'cucumber-html-formatter'
  s.version     = version
  s.authors     = ["Vincent Prêtre"]
  s.description = 'HTML formatter for Cucumber'
  s.summary     = "#{s.name}-#{s.version}"
  s.email       = 'cukes@googlegroups.com'
  s.homepage    = "https://github.com/cucumber/html-formatter"
  s.platform    = Gem::Platform::RUBY
  s.license     = "MIT"
  s.required_ruby_version = '>= 2.6'

  s.metadata    = {
                    'bug_tracker_uri'   => 'https://github.com/cucumber/html-formatter/issues',
                    'changelog_uri'     => 'https://github.com/cucumber/html-formatter/blob/main/CHANGELOG.md',
                    'documentation_uri' => 'https://github.com/cucumber/html-formatter',
                    'homepage_uri'      => s.homepage,
                    'mailing_list_uri'  => 'https://groups.google.com/forum/#!forum/cukes',
                    'source_code_uri'   => 'https://github.com/cucumber/html-formatter'
                  }

  s.add_runtime_dependency 'cucumber-messages', '> 19', '< 24'

  s.add_development_dependency 'rake', '~> 13.1'
  s.add_development_dependency 'rspec', '~> 3.12'
  s.add_development_dependency 'cucumber-compatibility-kit', '~> 14.1'

  s.executables      = ['cucumber-html-formatter']
  s.rubygems_version = ">= 3.0.3"
  s.files            = Dir['README.md', 'LICENSE', 'lib/**/*', 'assets/*']
  s.test_files       = Dir['spec/**/*']
  s.rdoc_options     = ["--charset=UTF-8"]
  s.require_path     = "lib"
end
