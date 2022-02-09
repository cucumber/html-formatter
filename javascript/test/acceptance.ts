import { NdjsonToMessageStream } from '@cucumber/message-streams'
import assert from 'assert'
import fs from 'fs'
import glob from 'glob'
import path from 'path'
import puppeteer from 'puppeteer'
import { pipeline } from 'stream'

import CucumberHtmlStream from '../src/CucumberHtmlStream'

async function canRenderHtml(html: string): Promise<boolean> {
  const browser = await puppeteer.launch({
    headless: true,
    args: ['--no-sandbox'],
  })
  const page = await browser.newPage()
  await page.setContent(html)
  const dynamicHTML = await page.evaluate(() => {
    const content = document.querySelector('[data-testid="cucumber-react"]')
    return content && content.innerHTML
  })
  await browser.close()

  if (!dynamicHTML) {
    return false
  }

  return true
}

describe('html-formatter', () => {
  const files = glob.sync(
    `./node_modules/@cucumber/compatibility-kit/features/**/*.ndjson`
  )
  for (const ndjson of files) {
    it(`can render ${path.basename(ndjson, '.ndjson')}`, async () => {
      const ndjsonData = fs.readFileSync(ndjson, { encoding: 'utf-8' })
      const toMessageStream = new NdjsonToMessageStream()
      let htmlData = Buffer.from('')
      await new Promise((resolve, reject) => {
        pipeline(
          ndjsonData,
          toMessageStream,
          new CucumberHtmlStream(
            __dirname + '/../dist/main.css',
            __dirname + '/../dist/main.js'
          ),
          (err: Error) => {
            if (err) {
              reject(err)
            }
          }
        )
          .on('data', (chunk) => {
            htmlData = Buffer.concat([htmlData, Buffer.from(chunk)])
          })
          .on('end', resolve)
      })

      assert.ok(await canRenderHtml(htmlData.toString()))
    })
  }
})
