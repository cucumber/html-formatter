package io.cucumber.htmlformatter;

import io.cucumber.htmlformatter.MessagesToHtmlWriter.Serializer;
import io.cucumber.messages.NdjsonToMessageIterable;
import io.cucumber.messages.NdjsonToMessageIterable.Deserializer;
import io.cucumber.messages.types.Envelope;
import org.jspecify.annotations.NullMarked;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import static io.cucumber.htmlformatter.Jackson.OBJECT_MAPPER;

public final class Main {
    private static final Deserializer deserializer = json -> OBJECT_MAPPER.readValue(json, Envelope.class);
    private static final Serializer serializer = OBJECT_MAPPER::writeValue;

    private Main() {
        // main class
    }

    public static void main(String[] args) throws IOException {
        InputStream in;
        if (args.length != 1) {
            in = new NonClosableInputStream(System.in);
        } else {
            in = new FileInputStream(args[0]);
        }
        try (NdjsonToMessageIterable envelopes = new NdjsonToMessageIterable(in, deserializer)) {
            MessagesToHtmlWriter.Builder builder = MessagesToHtmlWriter.builder(serializer);
            OutputStream out = new NonClosableOutputStream(System.out);
            try (MessagesToHtmlWriter htmlWriter = builder.build(out)) {
                for (Envelope envelope : envelopes) {
                    htmlWriter.write(envelope);
                }
            }
        } catch (Throwable e) {
            // Workaround for https://github.com/mojohaus/exec-maven-plugin/issues/141
            e.printStackTrace();
            System.exit(1);
        }
    }

    @NullMarked
    private static class NonClosableInputStream extends InputStream {

        private final InputStream delegate;

        NonClosableInputStream(InputStream delegate) {
            this.delegate = delegate;
        }

        @Override
        public int read() throws IOException {
            return delegate.read();
        }

        @Override
        public int read(byte[] b, int off, int len) throws IOException {
            return delegate.read(b, off, len);
        }
    }

    @NullMarked
    private static class NonClosableOutputStream extends OutputStream {

        private final OutputStream delegate;

        NonClosableOutputStream(OutputStream delegate) {
            this.delegate = delegate;
        }

        @Override
        public void write(int b) throws IOException {
            delegate.write(b);
        }

        @Override
        public void write(byte[] b, int off, int len) throws IOException {
            delegate.write(b, off, len);
        }
    }
}
