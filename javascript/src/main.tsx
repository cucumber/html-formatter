import './styles.scss'

import {
  EnvelopesProvider,
  ExecutionSummary,
  FilteredDocuments,
  SearchBar,
  UrlSearchProvider,
} from '@cucumber/react-components'
import React from 'react'
import { createRoot } from 'react-dom/client'
import { Envelope } from '@cucumber/messages'

declare global {
  interface Window {
    CUCUMBER_MESSAGES: Envelope[]
  }
}

const root = createRoot(document.getElementById('content') as HTMLElement)

root.render(
  <EnvelopesProvider envelopes={window.CUCUMBER_MESSAGES}>
    <UrlSearchProvider>
      <div id="report" className="html-formatter">
        <div className="html-formatter__header">
          <ExecutionSummary />
          <SearchBar />
        </div>
        <FilteredDocuments />
      </div>
    </UrlSearchProvider>
  </EnvelopesProvider>
)
