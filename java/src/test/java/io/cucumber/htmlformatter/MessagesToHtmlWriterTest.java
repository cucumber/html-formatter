package io.cucumber.htmlformatter;

import io.cucumber.htmlformatter.MessagesToHtmlWriter.Serializer;
import io.cucumber.messages.types.Comment;
import io.cucumber.messages.types.Envelope;
import io.cucumber.messages.types.GherkinDocument;
import io.cucumber.messages.types.Location;
import io.cucumber.messages.types.TestRunFinished;
import io.cucumber.messages.types.TestRunStarted;
import io.cucumber.messages.types.Timestamp;
import org.junit.jupiter.api.Test;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.time.Instant;

import static io.cucumber.messages.Convertor.toMessage;
import static java.nio.charset.StandardCharsets.UTF_8;
import static java.util.Collections.singletonList;
import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.assertArrayEquals;
import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertThrows;

class MessagesToHtmlWriterTest {

    static final Serializer serializer = Jackson.OBJECT_MAPPER::writeValue;

    private static ByteArrayInputStream createInputStream(String s) {
        return new ByteArrayInputStream(s.getBytes(UTF_8));
    }

    private static String renderAsHtml(Envelope... messages) throws IOException {
        return renderAsHtml(MessagesToHtmlWriter.builder(serializer), messages);
    }

    private static String renderAsHtml(MessagesToHtmlWriter.Builder builder, Envelope... messages) throws IOException {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        try (MessagesToHtmlWriter messagesToHtmlWriter = builder.build(bytes)) {
            for (Envelope message : messages) {
                messagesToHtmlWriter.write(message);
            }
        }

        return bytes.toString(UTF_8);
    }

    @Test
    void it_writes_one_message_to_html() throws IOException {
        Instant timestamp = Instant.ofEpochSecond(10);
        Envelope envelope = Envelope.of(new TestRunStarted(toMessage(timestamp), null));
        String html = renderAsHtml(envelope);
        assertThat(html).contains("window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}}];");
    }

    @Test
    void it_writes_no_message_to_html() throws IOException {
        String html = renderAsHtml();
        assertThat(html).contains("window.CUCUMBER_MESSAGES = [];");
    }

    @Test
    void it_writes_default_title() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer));
        assertThat(html).contains("<title>Cucumber</title>");
    }

    @Test
    void it_writes_custom_title() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer).title("Custom Title"));
        assertThat(html).contains("<title>Custom Title</title>");
    }

    @Test
    void it_writes_default_icon() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer));
        assertThat(html).contains("<link rel=\"icon\" href=\"data:image/svg+xml;base64,");
    }

    @Test
    void it_writes_custom_icon() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer)
                .icon("https://example.com/logo.svg"));
        assertThat(html).contains("<link rel=\"icon\" href=\"https://example.com/logo.svg\">");
    }

    @Test
    void it_writes_default_css() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer)
                .css(() -> createInputStream("p { color: red; }")));
        assertThat(html).contains("p { color: red; }");
    }

    @Test
    void it_writes_custom_css() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer)
                .customCss(() -> createInputStream("p { color: red; }")));
        assertThat(html).contains("p { color: red; }");
    }

    @Test
    void it_writes_default_script() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer)
                .script(() -> createInputStream("console.log(\"Hello world\");")));
        assertThat(html).contains("console.log(\"Hello world\");");
    }

    @Test
    void it_writes_custom_script() throws IOException {
        String html = renderAsHtml(MessagesToHtmlWriter.builder(serializer)
                .customScript(() -> createInputStream("console.log(\"Hello world\");")));
        assertThat(html).contains("console.log(\"Hello world\");");
    }

    @Test
    void it_throws_when_writing_after_close() throws IOException {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        MessagesToHtmlWriter messagesToHtmlWriter = MessagesToHtmlWriter.builder(serializer).build(bytes);
        messagesToHtmlWriter.close();
        assertThrows(IOException.class, () -> messagesToHtmlWriter.write(
                Envelope.of(new TestRunStarted(new Timestamp(0L, 0), ""))
        ));
    }

    @Test
    void it_can_be_closed_twice() throws IOException {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        MessagesToHtmlWriter messagesToHtmlWriter = MessagesToHtmlWriter.builder(serializer).build(bytes);
        messagesToHtmlWriter.close();
        assertDoesNotThrow(messagesToHtmlWriter::close);
    }

    @Test
    void it_is_idempotent_under_failure_to_close() {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream() {
            @Override
            public void close() throws IOException {
                throw new IOException("Can't close this");
            }
        };
        MessagesToHtmlWriter messagesToHtmlWriter = MessagesToHtmlWriter.builder(serializer).build(bytes);
        assertThrows(IOException.class, messagesToHtmlWriter::close);
        byte[] before = bytes.toByteArray();
        assertDoesNotThrow(messagesToHtmlWriter::close);
        byte[] after = bytes.toByteArray();
        assertArrayEquals(before, after);
    }

    @Test
    void it_writes_two_messages_separated_by_a_comma() throws IOException {
        Envelope testRunStarted = Envelope.of(new TestRunStarted(toMessage(Instant.ofEpochSecond(10)), null));

        Envelope envelope = Envelope.of(new TestRunFinished(null, true, toMessage(Instant.ofEpochSecond(15)), null, null));

        String html = renderAsHtml(testRunStarted, envelope);

        assertThat(html).contains("window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}},{\"testRunFinished\":{\"success\":true,\"timestamp\":{\"seconds\":15,\"nanos\":0}}}];");
    }

    @Test
    void it_escapes_opening_angle_bracket() throws IOException {
        Envelope envelope = Envelope.of(new GherkinDocument(
                null,
                null,
                singletonList(new Comment(
                        new Location(0, 0),
                        "</script><script>alert('Hello')</script>"
                ))
        ));
        String html = renderAsHtml(envelope);
        assertThat(html).contains(
                "window.CUCUMBER_MESSAGES = [{\"gherkinDocument\":{\"comments\":[{\"location\":{\"line\":0,\"column\":0},\"text\":\"\\x3C/script>\\x3Cscript>alert('Hello')\\x3C/script>\"}]}}];");
    }
}
