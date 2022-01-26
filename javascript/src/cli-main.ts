import { NdjsonToMessageStream } from '@cucumber/message-streams'
import commander from 'commander'
import { pipeline } from 'stream'

import p from '../package.json'
import CucumberHtmlStream from './CucumberHtmlStream'

const program = new commander.Command()

program.version(p.version)
program.parse(process.argv)

const toMessageStream = new NdjsonToMessageStream()
pipeline(
  process.stdin,
  toMessageStream,
  new CucumberHtmlStream(
    __dirname + '/../../dist/main.css',
    __dirname + '/../../dist/main.js'
  ),
  process.stdout,
  (err: Error) => {
    if (err) {
      // tslint:disable-next-line:no-console
      console.error(err)
      process.exit(1)
    }
  }
)
