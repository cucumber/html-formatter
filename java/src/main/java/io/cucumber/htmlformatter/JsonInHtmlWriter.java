package io.cucumber.htmlformatter;

import java.io.IOException;
import java.io.Writer;

/**
 * Writes json with the forward slash ({@code /}) escaped. Assumes
 * JSON has not been escaped yet.
 */
class JsonInHtmlWriter extends Writer {
    private static final int BUFFER_SIZE = 1024;
    private final Writer delegate;
    private char[] escapeBuffer;

    JsonInHtmlWriter(Writer delegate) {
        this.delegate = delegate;
    }

    @Override
    public void write(char[] source, int offset, int length) throws IOException {
        char[] destination = prepareBuffer();
        int flushAt = BUFFER_SIZE - 2;
        int written = 0;
        for (int i = offset; i < offset + length; i++) {
            char c = source[i];

            // Flush buffer if (nearly) full
            if (written >= flushAt) {
                delegate.write(destination, 0, written);
                written = 0;
            }

            // Write with escapes
            if (c == '/') {
                destination[written++] = '\\';
            }
            destination[written++] = c;
        }
        // Flush any remaining
        if (written > 0) {
            delegate.write(destination, 0, written);
        }
    }

    private char[] prepareBuffer() {
        // Reuse the same buffer, avoids repeated array allocation
        if (escapeBuffer == null) {
            escapeBuffer = new char[BUFFER_SIZE];
        }
        return escapeBuffer;
    }

    @Override
    public void flush() throws IOException {
        delegate.flush();
    }

    @Override
    public void close() throws IOException {
        delegate.close();
    }
}
