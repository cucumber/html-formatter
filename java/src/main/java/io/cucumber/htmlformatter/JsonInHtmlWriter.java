package io.cucumber.htmlformatter;

import java.io.IOException;
import java.io.Writer;

/**
 * Writes json with the forward slash ({@code /}) escaped.
 */
class JsonInHtmlWriter extends Writer {
    private static final int WRITE_BUFFER_SIZE = 1024;
    private final Writer delegate;
    private char[] writeBuffer;

    JsonInHtmlWriter(Writer delegate) {
        this.delegate = delegate;
    }

    @Override
    public void write(char[] buffer, int offset, int length) throws IOException {
        int escapes = countEscapes(buffer, offset, length);
        if (escapes == 0) {
            delegate.write(buffer, offset, length);
            return;
        }
        int escapedLength = length + escapes;
        char[] escapedBuffer = prepareWriteBuffer(escapedLength);
        writeEscapeTo(buffer, offset, length, escapedBuffer);
        delegate.write(escapedBuffer, 0, escapedLength);
    }

    private static int countEscapes(char[] source, int startAt, int length) {
        int count = 0;
        for (int i = startAt; i < startAt + length; i++) {
            if (source[i] == '/') {
                count++;
            }
        }
        return count;
    }

    private char[] prepareWriteBuffer(int length) {
        if (length > WRITE_BUFFER_SIZE) {
            return new char[length];
        }
        // Reuse the same write buffer, avoid array allocations
        if (writeBuffer == null) {
            writeBuffer = new char[WRITE_BUFFER_SIZE];
        }
        return writeBuffer;
    }

    private static void writeEscapeTo(char[] source, int startAt, int length, char[] destination) {
        for (int i = startAt, j = 0; i < startAt + length; i++) {
            char c = source[i];
            if (c == '/') {
                destination[j++] = '\\';
            }
            destination[j++] = c;
        }
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
