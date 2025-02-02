package io.cucumber.htmlformatter;

import io.cucumber.htmlformatter.MessagesToHtmlWriter.Serializer;
import io.cucumber.messages.Convertor;
import io.cucumber.messages.types.Comment;
import io.cucumber.messages.types.Envelope;
import io.cucumber.messages.types.Feature;
import io.cucumber.messages.types.GherkinDocument;
import io.cucumber.messages.types.Location;
import io.cucumber.messages.types.TestRunFinished;
import io.cucumber.messages.types.TestRunStarted;
import org.junit.jupiter.api.Test;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.time.Instant;
import java.util.Arrays;
import java.util.Collections;

import static java.nio.charset.StandardCharsets.UTF_8;
import static java.util.Collections.singletonList;
import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertArrayEquals;
import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertThrows;

class MessagesToHtmlWriterTest {

    static final Serializer serializer = Jackson.OBJECT_MAPPER::writeValue;

    @Test
    void it_writes_one_message_to_html() throws IOException {
        Instant timestamp = Instant.ofEpochSecond(10);
        Envelope envelope = Envelope.of(new TestRunStarted(Convertor.toMessage(timestamp), null));
        String html = renderAsHtml(envelope);
        assertThat(html, containsString("" +
                "window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}}];"));
    }

    @Test
    void it_writes_no_message_to_html() throws IOException {
        String html = renderAsHtml();
        assertThat(html, containsString("window.CUCUMBER_MESSAGES = [];"));
    }


    @Test
    void it_throws_when_writing_after_close() throws IOException {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer);
        messagesToHtmlWriter.close();
        assertThrows(IOException.class, () -> messagesToHtmlWriter.write(null));
    }

    @Test
    void it_can_be_closed_twice() throws IOException {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer);
        messagesToHtmlWriter.close();
        assertDoesNotThrow(messagesToHtmlWriter::close);
    }

    @Test
    void it_is_idempotent_under_failure_to_close() throws IOException {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream() {
            @Override
            public void close() throws IOException {
                throw new IOException("Can't close this");
            }
        };
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer);
        assertThrows(IOException.class, messagesToHtmlWriter::close);
        byte[] before = bytes.toByteArray();
        assertDoesNotThrow(messagesToHtmlWriter::close);
        byte[] after = bytes.toByteArray();
        assertArrayEquals(before, after);
    }

    @Test
    void it_writes_two_messages_separated_by_a_comma() throws IOException {
        Envelope testRunStarted = Envelope.of(new TestRunStarted(Convertor.toMessage(Instant.ofEpochSecond(10)), null));

        Envelope envelope = Envelope.of(new TestRunFinished(null, true, Convertor.toMessage(Instant.ofEpochSecond(15)), null, null));

        String html = renderAsHtml(testRunStarted, envelope);

        assertThat(html, containsString("" +
                "window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}},{\"testRunFinished\":{\"success\":true,\"timestamp\":{\"seconds\":15,\"nanos\":0}}}];"));
    }


    @Test
    void it_escapes_forward_slashes() throws IOException {
        Envelope envelope = Envelope.of(new GherkinDocument(
                null,
                null,
                singletonList(new Comment(
                        new Location(0L, 0L),
                        "</script><script>alert('Hello')</script>"
                ))
        ));
        String html = renderAsHtml(envelope);
        assertThat(html, containsString(
                "window.CUCUMBER_MESSAGES = [{\"gherkinDocument\":{\"comments\":[{\"location\":{\"line\":0,\"column\":0},\"text\":\"<\\/script><script>alert('Hello')<\\/script>\"}]}}];"));
    }

    private static String renderAsHtml(Envelope... messages) throws IOException {
        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        try (MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer)) {
            for (Envelope message : messages) {
                messagesToHtmlWriter.write(message);
            }
        }

        return new String(bytes.toByteArray(), UTF_8);
    }
}
