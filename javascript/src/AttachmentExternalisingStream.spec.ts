import * as messages from '@cucumber/messages'
import assert from 'assert'
import fs from 'fs'
import os from 'os'
import path from 'path'
import { Writable } from 'stream'

import { AttachmentExternalisingStream } from './AttachmentExternalisingStream'

async function collectMessages(
  envelopes: messages.Envelope[],
  directory: string
): Promise<messages.Envelope[]> {
  return new Promise((resolve, reject) => {
    const result: messages.Envelope[] = []
    const sink = new Writable({
      objectMode: true,
      write(chunk, _, callback) {
        result.push(chunk)
        callback()
      },
    })
    sink.on('finish', () => resolve(result))
    const stream = new AttachmentExternalisingStream(directory)
    stream.on('error', reject)
    stream.pipe(sink)
    for (const envelope of envelopes) {
      stream.write(envelope)
    }
    stream.end()
  })
}

function getAttachment(envelope: messages.Envelope): messages.Attachment {
  assert.ok(envelope.attachment, 'expected envelope to have an attachment')
  return envelope.attachment
}

describe('AttachmentExternalisingStream', () => {
  let tmpDir: string

  beforeEach(() => {
    tmpDir = fs.mkdtempSync(path.join(os.tmpdir(), 'cucumber-html-'))
  })

  afterEach(() => {
    fs.rmSync(tmpDir, { recursive: true })
  })

  it('passes through non-attachment messages unchanged', async () => {
    const envelope: messages.Envelope = {
      testRunStarted: { timestamp: { seconds: 0, nanos: 0 } },
    }
    const result = await collectMessages([envelope], tmpDir)
    assert.deepStrictEqual(result, [envelope])
  })

  it('externalises a BASE64 attachment', async () => {
    const body = Buffer.from('hello').toString('base64')
    const envelope: messages.Envelope = {
      attachment: {
        body,
        contentEncoding: messages.AttachmentContentEncoding.BASE64,
        mediaType: 'image/png',
      },
    }
    const result = await collectMessages([envelope], tmpDir)
    const attachment = getAttachment(result[0])
    assert.strictEqual(attachment.body, '')
    assert.strictEqual(
      attachment.contentEncoding,
      messages.AttachmentContentEncoding.IDENTITY
    )
    assert.ok(attachment.url)
    assert.match(attachment.url, /^\.\/attachment-.*\.png$/)
    const written = fs.readFileSync(path.join(tmpDir, attachment.url.slice(2)))
    assert.deepStrictEqual(written, Buffer.from('hello'))
  })

  it('externalises an IDENTITY attachment', async () => {
    const envelope: messages.Envelope = {
      attachment: {
        body: '{"some":"json"}',
        contentEncoding: messages.AttachmentContentEncoding.IDENTITY,
        mediaType: 'application/json',
      },
    }
    const result = await collectMessages([envelope], tmpDir)
    const attachment = getAttachment(result[0])
    assert.strictEqual(attachment.body, '')
    assert.ok(attachment.url)
    assert.match(attachment.url, /^\.\/attachment-.*\.json$/)
    const written = fs.readFileSync(
      path.join(tmpDir, attachment.url.slice(2)),
      'utf-8'
    )
    assert.strictEqual(written, '{"some":"json"}')
  })

  it('does not externalise text/x.cucumber.log+plain', async () => {
    const envelope: messages.Envelope = {
      attachment: {
        body: 'some log output',
        contentEncoding: messages.AttachmentContentEncoding.IDENTITY,
        mediaType: 'text/x.cucumber.log+plain',
      },
    }
    const result = await collectMessages([envelope], tmpDir)
    const attachment = getAttachment(result[0])
    assert.strictEqual(attachment.body, 'some log output')
    assert.strictEqual(attachment.url, undefined)
  })

  it('does not externalise text/uri-list', async () => {
    const envelope: messages.Envelope = {
      attachment: {
        body: 'https://example.com',
        contentEncoding: messages.AttachmentContentEncoding.IDENTITY,
        mediaType: 'text/uri-list',
      },
    }
    const result = await collectMessages([envelope], tmpDir)
    const attachment = getAttachment(result[0])
    assert.strictEqual(attachment.body, 'https://example.com')
    assert.strictEqual(attachment.url, undefined)
  })
})
