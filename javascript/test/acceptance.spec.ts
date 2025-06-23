import { NdjsonToMessageStream } from '@cucumber/message-streams'
import { expect, test } from '@playwright/test'
import fs from 'fs'
import { sync } from 'glob'
import path from 'path'
import { PassThrough, pipeline } from 'stream'

import { CucumberHtmlStream } from '../src'

const fixtures = sync(
  `./node_modules/@cucumber/compatibility-kit/features/**/*.ndjson`
)

test.beforeAll(async () => {
  const acceptanceDir = path.join(__dirname, '../acceptance')

  for (const fixture of fixtures) {
    const name = path.basename(fixture, '.ndjson')
    const ndjsonData = fs.createReadStream(fixture, { encoding: 'utf-8' })
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
      path.join(acceptanceDir, name + '.html'),
      htmlData.toString(),
      { encoding: 'utf-8' }
    )
  }
})

for (const fixture of fixtures) {
  const name = path.basename(fixture, '.ndjson')

  test(`can render ${name}`, async ({ page }) => {
    await page.goto(`/${name}.html`)
    await page.waitForSelector('#report', { timeout: 3000 })
    await expect(page).toHaveScreenshot(`${name}.png`, {
      maxDiffPixelRatio: 0.05,
    })
  })
}
