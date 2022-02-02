import org.junit.jupiter.api.Test;

import java.lang.InterruptedException;
import java.lang.ProcessBuilder;
import java.lang.ProcessBuilder.Redirect;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.InputStreamReader;
import java.io.File;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

import static java.nio.file.Files.newDirectoryStream;

class HtmlFormatterAcceptanceTest {

    @Test
    void it_works() throws IOException {
        List<Path> paths = new ArrayList<>();

        newDirectoryStream(
            Paths.get("..", "javascript", "node_modules", "@cucumber", "compatibility-kit", "features")
        ).forEach((path) -> {
            String featureName = path.getFileName().toString();
            try {
                newDirectoryStream(
                    Paths.get(path.toString()),
                    "*.ndjson"
                ).forEach((ndjsonFilePath) -> {

                    try {
                        InputStream inputStream = Files.newInputStream(ndjsonFilePath);

                        ProcessBuilder pb = new ProcessBuilder("mvn --quiet --batch-mode exec:java -Dexec.mainClass=io.cucumber.htmlformatter.Main")
                                .redirectInput(new File(ndjsonFilePath.toString()))
                                .redirectOutput(Redirect.INHERIT)
                                .redirectError(Redirect.INHERIT);

                        Process p = pb.start();
                        int rc = p.waitFor();

                        assertEquals(0, rc);
                    } catch (IOException ioe) {
                        // throw ioe;
                    } catch (InterruptedException ie) {
                        //
                    }
                });
            } catch (IOException ioe) {
                // throw ioe;
            }

        });
    }

}
