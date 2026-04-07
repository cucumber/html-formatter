import {
  Attachment,
  AttachmentContentEncoding,
  Envelope,
  IdGenerator,
} from '@cucumber/messages'
import fs from 'fs'
import mime from 'mime'
import path from 'path'
import { Transform, TransformCallback } from 'stream'

const alwaysInlinedTypes = ['text/x.cucumber.log+plain', 'text/uri-list']

const encodingsMap: Record<string, BufferEncoding> = {
  IDENTITY: 'utf-8',
  BASE64: 'base64',
}

export class AttachmentExternalisingStream extends Transform {
  private readonly writeOperations: Promise<void>[] = []

  constructor(
    private readonly directory: string,
    private readonly newId: IdGenerator.NewId = IdGenerator.uuid()
  ) {
    super({ objectMode: true })
  }

  public _transform(
    envelope: Envelope,
    encoding: string,
    callback: TransformCallback
  ): void {
    if (envelope.attachment) {
      const { attachment, writeOperation } = rewriteAttachment(
        envelope.attachment,
        this.directory,
        this.newId
      )
      this.push({ ...envelope, attachment })
      if (writeOperation) {
        this.writeOperations.push(writeOperation)
      }
    } else {
      this.push(envelope)
    }
    callback()
  }

  public _flush(callback: TransformCallback): void {
    Promise.all(this.writeOperations).then(
      () => callback(),
      (err) => callback(err)
    )
  }
}

function rewriteAttachment(
  original: Attachment,
  directory: string,
  newId: () => string
): { attachment: Attachment; writeOperation?: Promise<void> } {
  if (alwaysInlinedTypes.includes(original.mediaType)) {
    return { attachment: original }
  }
  let filename = `attachment-${newId()}`
  const extension = mime.getExtension(original.mediaType)
  if (extension) {
    filename += `.${extension}`
  }
  const writeOperation = fs.promises.writeFile(
    path.join(directory, filename),
    Buffer.from(original.body, encodingsMap[original.contentEncoding])
  )
  const attachment: Attachment = {
    ...original,
    contentEncoding: AttachmentContentEncoding.IDENTITY,
    body: '',
    url: `./${filename}`,
  }
  return { attachment, writeOperation }
}
