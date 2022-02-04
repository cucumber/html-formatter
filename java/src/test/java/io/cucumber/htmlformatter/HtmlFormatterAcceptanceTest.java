import org.junit.jupiter.api.Test;

import java.lang.InterruptedException;
import java.lang.ProcessBuilder;
import java.lang.ProcessBuilder.Redirect;
import java.io.IOException;
import java.io.File;
import java.nio.file.Paths;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

import static java.nio.file.Files.newDirectoryStream;

class HtmlFormatterAcceptanceTest {

    @Test
    void it_works() throws IOException {
        String features = "../javascript/node_modules/@cucumber/compatibility-kit/features";

        newDirectoryStream(Paths.get(features)).forEach((path) -> {

            try {

                newDirectoryStream(Paths.get(path.toString()), "*.ndjson").forEach((ndjsonFilePath) -> {

                    try {

                        ProcessBuilder pb =
                            new ProcessBuilder("mvn --quiet --batch-mode exec:java -Dexec.mainClass=io.cucumber.htmlformatter.Main")
                                .redirectInput(new File(ndjsonFilePath.toString()))
                                .redirectOutput(Redirect.INHERIT)
                                .redirectError(Redirect.INHERIT);

                        Process p = pb.start();
                        int rc = p.waitFor();

                        assertEquals(0, rc);

                    } catch (IOException ioe) {
                    } catch (InterruptedException ie) {
                    }

                });

            } catch (IOException ioe) {
            }
        });
    }
}
