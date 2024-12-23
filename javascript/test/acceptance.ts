import { NdjsonToMessageStream } from '@cucumber/message-streams'
import assert from 'assert'
import fs from 'fs'
import { sync } from 'glob'
import path from 'path'
import puppeteer from 'puppeteer'
import { PassThrough, pipeline } from 'stream'

import { CucumberHtmlStream } from '../src'

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
  const files = sync(
    `./node_modules/@cucumber/compatibility-kit/features/**/*.ndjson`
  )
  for (const ndjson of files) {
    const name = path.basename(ndjson, '.ndjson')
    it(`can render ${name}`, async () => {
      const ndjsonData = fs.createReadStream(ndjson, { encoding: 'utf-8' })
      const toMessageStream = new NdjsonToMessageStream()
      const htmlData = await new Promise<string>((resolve, reject) => {
        const chunks: Buffer[] = []
        const out = new PassThrough()
          .on('data', (chunk) => chunks.push(Buffer.from(chunk)))
          .on('end', () => resolve(Buffer.concat(chunks).toString()))
        pipeline(
          ndjsonData,
          toMessageStream,
          new CucumberHtmlStream(
            path.join(__dirname, '../dist/main.css'),
            path.join(__dirname, '../dist/main.js')
          ),
          out,
          (err: Error) => {
            if (err) {
              reject(err)
            }
          }
        )
      })
      fs.writeFileSync(
        path.join(__dirname, '../acceptance', name + '.html'),
        htmlData.toString(),
        { encoding: 'utf-8' }
      )
      assert.ok(await canRenderHtml(htmlData.toString()))
    })
  }
})
