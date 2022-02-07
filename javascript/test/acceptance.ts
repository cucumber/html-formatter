import assert from 'assert'
import { exec } from 'child_process'
import glob from 'glob'
import path from 'path'
import puppeteer from 'puppeteer'
import util from 'util'

const run = util.promisify(exec)

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
  for (const ndjson of glob.sync(
    `./node_modules/@cucumber/compatibility-kit/features/**/*.ndjson`
  )) {
    it(`can render ${path.basename(ndjson, '.ndjson')}`, async () => {
      // const ndjsonData = fs.readFileSync(ndjson, { encoding: 'utf-8' })
      const { stdout: htmlData } = await run(
        `npx shx cat ${ndjson} | node ./bin/cucumber-html-formatter.js`,
        { maxBuffer: 4096 * 1024 }
      )

      assert.ok(await canRenderHtml(htmlData))
    })
  }
})
