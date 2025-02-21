import './styles.scss'

import * as messages from '@cucumber/messages'
import { components, searchFromURLParams } from '@cucumber/react-components'
import React from 'react'
import { createRoot } from 'react-dom/client'

const { CucumberReact } = components
const { FilteredResults, EnvelopesWrapper, SearchWrapper } = components.app

declare global {
  interface Window {
    CUCUMBER_MESSAGES: messages.Envelope[]
  }
}

const root = createRoot(document.getElementById('content') as HTMLElement)

root.render(
  <CucumberReact theme="auto">
    <EnvelopesWrapper envelopes={window.CUCUMBER_MESSAGES}>
      <SearchWrapper {...searchFromURLParams()}>
        <FilteredResults className="html-formatter" />
      </SearchWrapper>
    </EnvelopesWrapper>
  </CucumberReact>
)
