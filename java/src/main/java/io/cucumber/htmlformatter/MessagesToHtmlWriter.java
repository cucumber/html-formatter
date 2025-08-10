package io.cucumber.htmlformatter;

import io.cucumber.messages.types.Envelope;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.nio.charset.StandardCharsets;
import java.util.function.Supplier;

import static java.nio.charset.StandardCharsets.UTF_8;
import static java.util.Objects.requireNonNull;

/**
 * Writes the message output of a test run as single page html report.
 */
public final class MessagesToHtmlWriter implements AutoCloseable {
    private final OutputStreamWriter writer;
    private final JsonInHtmlWriter jsonInHtmlWriter;
    private final Serializer serializer;

    private final String template;
    private final Supplier<InputStream> title;
    private final Supplier<InputStream> icon;
    private final Supplier<InputStream> css;
    private final Supplier<InputStream> customCss;
    private final Supplier<InputStream> script;
    private final Supplier<InputStream> customScript;

    private boolean preMessageWritten = false;
    private boolean postMessageWritten = false;
    private boolean firstMessageWritten = false;
    private boolean streamClosed = false;

    @Deprecated
    public MessagesToHtmlWriter(OutputStream outputStream, Serializer serializer) throws IOException {
        this(
                createWriter(outputStream),
                requireNonNull(serializer),
                () -> new ByteArrayInputStream("Cucumber".getBytes(UTF_8)),
                () -> getResource("icon.url"),
                () -> getResource("main.css"),
                MessagesToHtmlWriter::getEmptyResource,
                () -> getResource("main.js"),
                MessagesToHtmlWriter::getEmptyResource
        );
    }

    private MessagesToHtmlWriter(
            OutputStreamWriter writer,
            Serializer serializer,
            Supplier<InputStream> title,
            Supplier<InputStream> icon,
            Supplier<InputStream> css,
            Supplier<InputStream> customCss,
            Supplier<InputStream> script,
            Supplier<InputStream> customScript
    ) {
        this.writer = writer;
        this.jsonInHtmlWriter = new JsonInHtmlWriter(writer);
        this.serializer = serializer;
        this.template = readTemplate();
        this.title = title;
        this.icon = icon;
        this.css = css;
        this.customCss = customCss;
        this.customScript = customScript;
        this.script = script;
    }

    private static String readTemplate() {
        try {
            return readResource("index.mustache.html");
        } catch (IOException e) {
            throw new RuntimeException("Could not read resource index.mustache.html", e);
        }
    }

    private static OutputStreamWriter createWriter(OutputStream outputStream) {
        return new OutputStreamWriter(
                requireNonNull(outputStream),
                StandardCharsets.UTF_8);
    }

    /**
     * Creates a builder to construct this writer.
     *
     * @param serializer used to convert messages into json.
     * @return a new builder
     */
    public static Builder builder(Serializer serializer) {
        return new Builder(serializer);
    }

    private static ByteArrayInputStream getEmptyResource() {
        return new ByteArrayInputStream(new byte[0]);
    }

    private static void writeTemplateBetween(Writer writer, String template, String begin, String end)
            throws IOException {
        int beginIndex = begin == null ? 0 : template.indexOf(begin) + begin.length();
        int endIndex = end == null ? template.length() : template.indexOf(end);
        writer.write(template.substring(beginIndex, endIndex));
    }

    private static void writeResource(Writer writer, Supplier<InputStream> resource) throws IOException {
        writeResource(writer, resource.get());
    }

    private static void writeResource(Writer writer, InputStream resource) throws IOException {
        BufferedReader reader = new BufferedReader(new InputStreamReader(resource, UTF_8));
        char[] buffer = new char[1024];
        for (int read = reader.read(buffer); read != -1; read = reader.read(buffer)) {
            writer.write(buffer, 0, read);
        }
    }

    private static InputStream getResource(String name) {
        InputStream resource = MessagesToHtmlWriter.class.getResourceAsStream(name);
        requireNonNull(resource, name + " could not be loaded");
        return resource;
    }

    private static String readResource(String name) throws IOException {
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        try (BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(baos, UTF_8))) {
            InputStream resource = getResource(name);
            writeResource(writer, resource);
        }
        return new String(baos.toByteArray(), UTF_8);
    }

    private void writePreMessage() throws IOException {
        writeTemplateBetween(writer, template, null, "{{title}}");
        writeResource(writer, title);
        writeTemplateBetween(writer, template, "{{title}}", "{{icon}}");
        writeResource(writer, icon);
        writeTemplateBetween(writer, template, "{{icon}}", "{{css}}");
        writeResource(writer, css);
        writeTemplateBetween(writer, template, "{{css}}", "{{customCss}}");
        writeResource(writer, customCss);
        writeTemplateBetween(writer, template, "{{customCss}}", "{{messages}}");
    }

    private void writePostMessage() throws IOException {
        writeTemplateBetween(writer, template, "{{messages}}", "{{script}}");
        writeResource(writer, script);
        writeTemplateBetween(writer, template, "{{script}}", "{{customScript}}");
        writeResource(writer, customScript);
        writeTemplateBetween(writer, template, "{{customScript}}", null);
    }

    /**
     * Writes a cucumber message to the html output.
     *
     * @param envelope the message
     * @throws IOException if an IO error occurs
     */
    public void write(Envelope envelope) throws IOException {
        if (streamClosed) {
            throw new IOException("Stream closed");
        }

        if (!preMessageWritten) {
            writePreMessage();
            preMessageWritten = true;
        }

        if (!firstMessageWritten) {
            firstMessageWritten = true;
        } else {
            writer.write(",");
        }

        serializer.writeValue(jsonInHtmlWriter, envelope);
    }

    /**
     * Closes the stream, flushing it first. Once closed further write()
     * invocations will cause an IOException to be thrown. Closing a closed
     * stream has no effect.
     *
     * @throws IOException if an IO error occurs
     */
    @Override
    public void close() throws IOException {
        if (streamClosed) {
            return;
        }

        if (!preMessageWritten) {
            writePreMessage();
            preMessageWritten = true;
        }
        // writer.close may fail
        // this conditional keeps the writer idempotent
        if (!postMessageWritten) {
            writePostMessage();
            postMessageWritten = true;
        }
        try {
            writer.close();
        } finally {
            streamClosed = true;
        }
    }

    /**
     * Serializes a message to JSON.
     */
    @FunctionalInterface
    public interface Serializer {

        /**
         * Serialize a message to JSON and write it to the given {@code writer}.
         *
         * <ul>
         *     <li>Values must be included unless their value is {@code null}
         *     or an "absent" reference values such as empty optionals.
         *     <li>Enums must be written as strings.
         *     <li>The solidus {@code /} may not be escaped. Writing json
         *     into the html context is handled in this implementation.
         *     <li>Implementations may not close the {@code writer} after
         *     writing a {@code value}.
         * </ul>
         *
         * @param writer to write to
         * @param value  to serialize
         * @throws IOException if anything goes wrong
         */
        void writeValue(Writer writer, Envelope value) throws IOException;

    }

    public static final class Builder {
        private final Serializer serializer;
        private Supplier<InputStream> title = () -> new ByteArrayInputStream("Cucumber".getBytes(UTF_8));
        private Supplier<InputStream> icon = () -> getResource("icon.url");
        private Supplier<InputStream> css = () -> getResource("main.css");
        private Supplier<InputStream> customCss = MessagesToHtmlWriter::getEmptyResource;
        private Supplier<InputStream> script = () -> getResource("main.js");
        private Supplier<InputStream> customScript = MessagesToHtmlWriter::getEmptyResource;

        private Builder(Serializer serializer) {
            this.serializer = requireNonNull(serializer);
        }

        /**
         * Sets a custom title for the report, default value "Cucumber".
         *
         * @param title the custom title.
         * @return this builder
         */
        public Builder title(String title) {
            requireNonNull(title);
            this.title = () -> new ByteArrayInputStream(title.getBytes(UTF_8));
            return this;
        }

        /**
         * Sets a custom icon for the report, default value the cucumber logo.
         * <p>
         * The {@code icon} is any valid {@code href} value.
         *
         * @param icon the custom icon.
         * @return this builder
         */
        public Builder icon(Supplier<InputStream> icon) {
            this.icon = requireNonNull(icon);
            return this;
        }

        /**
         * Sets default css for the report.
         * <p>
         * The default script styles the cucumber report.
         *
         * @param css the custom css.
         * @return this builder
         */
        public Builder css(Supplier<InputStream> css) {
            this.css = requireNonNull(css);
            return this;
        }

        /**
         * Sets custom css for the report.
         * <p>
         * The custom css is applied after the default css.
         *
         * @param customCss the custom css.
         * @return this builder
         */
        public Builder customCss(Supplier<InputStream> customCss) {
            this.customCss = requireNonNull(customCss);
            return this;
        }

        /**
         * Replaces default script for the report.
         * <p>
         * The default script renders the cucumber messages into a report.
         *
         * @param script the custom script.
         * @return this builder
         */
        public Builder script(Supplier<InputStream> script) {
            this.script = requireNonNull(script);
            return this;
        }

        /**
         * Sets custom script for the report.
         * <p>
         * The custom script is applied after the default script.
         *
         * @param customScript the custom script.
         * @return this builder
         */
        public Builder customScript(Supplier<InputStream> customScript) {
            this.customScript = requireNonNull(customScript);
            return this;
        }


        /**
         * Create an instance of the messages to html writer.
         *
         * @param out the output stream to write to
         * @return a new instance of the messages to html writer.
         */
        public MessagesToHtmlWriter build(OutputStream out) {
            return new MessagesToHtmlWriter(createWriter(out), serializer, title, icon, css, customCss, script, customScript);
        }
    }

}
