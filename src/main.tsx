import { messages } from '@cucumber/messages'
import {
  GherkinDocumentList,
  QueriesWrapper,
  EnvelopesQuery,
} from '@cucumber/react'
import { Query as GherkinQuery } from '@cucumber/gherkin'
import { Query as CucumberQuery } from '@cucumber/query'
import React from 'react'
import ReactDOM from 'react-dom'

declare global {
  interface Window {
    CUCUMBER_MESSAGES: messages.IEnvelope[]
  }
}

const gherkinQuery = new GherkinQuery()
const cucumberQuery = new CucumberQuery()
const envelopesQuery = new EnvelopesQuery()

for (const envelopeObject of window.CUCUMBER_MESSAGES) {
  const envelope = messages.Envelope.fromObject(envelopeObject)
  gherkinQuery.update(envelope)
  cucumberQuery.update(envelope)
  envelopesQuery.update(envelope)
}

const app = (
  <QueriesWrapper
    gherkinQuery={gherkinQuery}
    cucumberQuery={cucumberQuery}
    envelopesQuery={envelopesQuery}
  >
    <GherkinDocumentList />
  </QueriesWrapper>
)

ReactDOM.render(app, document.getElementById('content'))
