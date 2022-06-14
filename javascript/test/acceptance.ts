import { NdjsonToMessageStream } from '@cucumber/message-streams'
import assert from 'assert'
import fs from 'fs'
import glob from 'glob'
import path from 'path'
import puppeteer from 'puppeteer'
import { PassThrough, pipeline } from 'stream'

import CucumberHtmlStream from '../src/CucumberHtmlStream'
import { writeFile } from 'fs/promises'

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
    const basename = path.basename(ndjson, '.ndjson')
    it(`can render ${basename}`, async () => {
      const ndjsonData = fs.createReadStream(ndjson, { encoding: 'utf-8' })
      const toMessageStream = new NdjsonToMessageStream()
      const html = await new Promise<string>((resolve, reject) => {
        const chunks: Buffer[] = []
        const out = new PassThrough()
          .on('data', (chunk) => chunks.push(Buffer.from(chunk)))
          .on('end', () => resolve(Buffer.concat(chunks).toString()))
        pipeline(
          ndjsonData,
          toMessageStream,
          new CucumberHtmlStream(
            __dirname + '/../dist/main.css',
            __dirname + '/../dist/main.js'
          ),
          out,
          (err: Error) => {
            if (err) {
              reject(err)
            }
          }
        )
      })
      await writeFile(`html/${basename}.html`, html, 'utf-8')
      assert.ok(await canRenderHtml(html))
    })
  }
})
