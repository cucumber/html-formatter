package io.cucumber.htmlformatter;

import org.junit.jupiter.api.Test;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.util.Arrays;

import static java.nio.charset.StandardCharsets.UTF_8;
import static org.junit.jupiter.api.Assertions.assertEquals;

class JsonInHtmlWriterTest {

    private final ByteArrayOutputStream out = new ByteArrayOutputStream();
    private final OutputStreamWriter outputStreamWriter = new OutputStreamWriter(out, UTF_8);
    private final JsonInHtmlWriter writer = new JsonInHtmlWriter(outputStreamWriter);

    @Test
    void writes() throws IOException {
        writer.write("{\"hello\": \"world\"}");
        assertEquals("{\"hello\": \"world\"}", output());
    }

    @Test
    void escapes_single() throws IOException {
        writer.write("<");
        assertEquals("\\x3C", output());
    }

    @Test
    void escapes_multiple() throws IOException {
        writer.write("</script><script></script>");
        assertEquals("\\x3C/script>\\x3Cscript>\\x3C/script>", output());
    }

    @Test
    void partial_writes() throws IOException {
        char[] buffer = new char[100];
        String text = "</script><script></script>";

        text.getChars(0, 9, buffer, 0);
        writer.write(buffer, 0, 9);

        text.getChars(9, 17, buffer, 2);
        writer.write(buffer, 2, 8);

        text.getChars(17, 26, buffer, 4);
        writer.write(buffer, 4, 9);

        assertEquals("\\x3C/script>\\x3Cscript>\\x3C/script>", output());
    }

    @Test
    void large_writes_with_odd_boundaries() throws IOException {
        char[] buffer = new char[1024];
        // This forces the buffer to flush after every 1023 written characters.
        buffer[0] = 'a';
        Arrays.fill(buffer, 1, buffer.length, '<');
        writer.write(buffer);

        StringBuilder expected = new StringBuilder();
        expected.append("a");
        for (int i = 1; i < buffer.length; i++) {
            expected.append("\\x3C");
        }
        assertEquals(expected.toString(), output());
    }


    @Test
    void really_large_writes() throws IOException {
        char[] buffer = new char[2048];
        Arrays.fill(buffer, '<');
        writer.write(buffer);

        StringBuilder expected = new StringBuilder();
        for (int i = 0; i < buffer.length; i++) {
            expected.append("\\x3C");
        }
        assertEquals(expected.toString(), output());
    }

    @Test
    void empty_write() throws IOException {
        char[] buffer = new char[0];
        writer.write(buffer);
        assertEquals("", output());
    }

    private String output() throws IOException {
        writer.flush();
        return new String(out.toByteArray(), UTF_8);
    }
}
