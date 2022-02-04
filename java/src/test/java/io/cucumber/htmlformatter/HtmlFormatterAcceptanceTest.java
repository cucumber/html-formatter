import org.junit.jupiter.api.Test;

import java.io.CharArrayWriter;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.nio.file.Paths;

import io.cucumber.htmlformatter.MessagesToHtmlWriter;
import io.cucumber.messages.NdjsonToMessageIterable;
import io.cucumber.messages.types.Envelope;

import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.fail;

import static java.nio.file.Files.newDirectoryStream;

class HtmlFormatterAcceptanceTest {

    @Test
    void it_works() throws IOException {
        String features = "../javascript/node_modules/@cucumber/compatibility-kit/features";

        newDirectoryStream(Paths.get(features)).forEach((path) -> {

            try {

                newDirectoryStream(Paths.get(path.toString()), "*.ndjson").forEach((ndjsonFilePath) -> {

                    int renderedLength = this.renderHtmlFileForGivenNdjson(ndjsonFilePath.toString());

                    assertNotEquals(0, renderedLength);

                });

            } catch (IOException ioe) {
                fail();
            }
        });
    }

    int renderHtmlFileForGivenNdjson(String ndjsonFilePath) {
        try {
            CharArrayWriter writer = new CharArrayWriter();
            NdjsonToMessageIterable envelopes = new NdjsonToMessageIterable(new FileInputStream(ndjsonFilePath));
            MessagesToHtmlWriter htmlWriter = new MessagesToHtmlWriter(writer);

            for (Envelope envelope : envelopes) {
                htmlWriter.write(envelope);
            }

            return writer.size();
        } catch (FileNotFoundException fnfe) {
            return 0;
        } catch (IOException ioe) {
            return 0;
        }
    }

}
