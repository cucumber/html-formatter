using static System.Text.Encoding;
using Io.Cucumber.Messages.Types;
using Cucumber.Messages;
using Cucumber.HtmlFormatter;

namespace Cucumber.HtmlFormatterTest;

[TestClass]
public class MessagesToHtmlWriterTest
{
    private static readonly Action<StreamWriter, Envelope> Serializer = (sw, e) =>
    {
        var s = NdjsonSerializer.Serialize(e);
        sw.Write(s);
    };

    private static readonly Func<StreamWriter, Envelope, Task> AsyncSerializer = async (sw, e) =>
    {
        var s = NdjsonSerializer.Serialize(e);
        await sw.WriteAsync(s);
    };

    [TestMethod]
    public async Task LegacySyncApiRendersTheSameAsAsync()
    {
        Envelope sampleEnvelope = Envelope.Create(new TestRunStarted(Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null));
        string expectedHtml = await RenderAsHtmlAsync(sampleEnvelope);

        // ReSharper disable once MethodHasAsyncOverload
        string html = RenderAsHtml(sampleEnvelope);

        Assert.AreEqual(expectedHtml, html, "Expected legacy sync API to render the same HTML as the async API.");
    }

    [TestMethod]
    public async Task ItWritesOneMessageToHtmlAsync()
    {
        DateTime timestamp = DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime();
        Envelope envelope = Envelope.Create(new TestRunStarted(Converters.ToTimestamp(timestamp), null));
        string html = await RenderAsHtmlAsync(envelope);
        StringAssert.Contains(html, "window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}}];",
            $"Expected html to contain a testRunStarted message, but instead html contained: {html}");
    }

    [TestMethod]
    public async Task ItWritesNoMessageToHtml()
    {
        string html = await RenderAsHtmlAsync();
        StringAssert.Contains(html, "window.CUCUMBER_MESSAGES = [];");
    }

    [TestMethod]
    public async Task ItWritesDefaultTitle() 
    {
        string html = await RenderAsHtmlAsync();
        StringAssert.Contains(html, "<title>Cucumber</title>");
    }

    [TestMethod]
    public async Task ItWritesCustomTitle()
    {
        string html = await RenderAsHtmlAsync(new HtmlReportSettings { Title = "Custom Title" });
        StringAssert.Contains(html, "<title>Custom Title</title>");
    }

    [TestMethod]
    public async Task ItWritesDefaultIcon()
    {
        string html = await RenderAsHtmlAsync();
        StringAssert.Contains(html, "<link rel=\"icon\" href=\"data:image/svg+xml;base64,");
    }

    [TestMethod]
    public async Task ItWritesCustomIcon()
    {
        string html = await RenderAsHtmlAsync(new HtmlReportSettings { Icon = "<link rel=\"icon\" href=\"https://example.com/logo.svg\">" });
        StringAssert.Contains(html, "<link rel=\"icon\" href=\"https://example.com/logo.svg\">");
    }

    [TestMethod]
    public async Task ItWritesCustomCss()
    {
        string html = await RenderAsHtmlAsync(new HtmlReportSettings { CustomCss = "p { color: red; }" });
        StringAssert.Contains(html, "p { color: red; }");
    }

    [TestMethod]
    public async Task ItWritesCustomScript()
    {
        string html = await RenderAsHtmlAsync(new HtmlReportSettings() { CustomScript = "console.log(\"Hello world\");" });
        StringAssert.Contains(html, "console.log(\"Hello world\");");
    }

    [TestMethod]
    public async Task ItWritesNoMessageToHtmlAsync()
    {
        string html = await RenderAsHtmlAsync();
        StringAssert.Contains(html, "window.CUCUMBER_MESSAGES = [];");
    }

    [TestMethod]
    public void ItThrowsWhenWritingAfterClose()
    {
        MemoryStream bytes = new MemoryStream();
#pragma warning disable CS0618 // Type or member is obsolete
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, Serializer);
#pragma warning restore CS0618 // Type or member is obsolete
        messagesToHtmlWriter.Dispose();
        Assert.ThrowsExactly<IOException>(() => messagesToHtmlWriter.Write(Envelope.Create(new TestRunStarted(Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null))));
    }

    [TestMethod]
    public async Task ItThrowsWhenWritingAfterCloseAsync()
    {
        MemoryStream bytes = new MemoryStream();
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, AsyncSerializer);
        await messagesToHtmlWriter.DisposeAsync();
        await Assert.ThrowsExactlyAsync<IOException>(async () =>
            await messagesToHtmlWriter.WriteAsync(
                Envelope.Create(new TestRunStarted(
                    Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null))));
    }

    [TestMethod]
    public void ItCanBeClosedTwice()
    {
        MemoryStream bytes = new MemoryStream();
#pragma warning disable CS0618 // Type or member is obsolete
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, Serializer);
#pragma warning restore CS0618 // Type or member is obsolete
        messagesToHtmlWriter.Dispose();
        try { messagesToHtmlWriter.Dispose(); }
        catch (System.Exception e)
        {
            throw new System.Exception($"Expected no exception, but got {e.Message}");
        }
    }

    [TestMethod]
    public async Task ItCanBeClosedTwiceAsync()
    {
        MemoryStream bytes = new MemoryStream();
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, AsyncSerializer);
        await messagesToHtmlWriter.DisposeAsync();
        try { await messagesToHtmlWriter.DisposeAsync(); }
        catch (System.Exception e)
        {
            throw new System.Exception($"Expected no exception, but got {e.Message}");
        }
    }

    internal class StreamThatFailsToClose : MemoryStream
    {
        public override void Close()
        {
            throw new System.Exception("Can't close this");
        }
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            throw new System.Exception("Can't flush this");
        }
    }

    [TestMethod]
    public void ItIsIdempotentUnderFailureToClose()
    {
        var bytes = new StreamThatFailsToClose();
#pragma warning disable CS0618 // Type or member is obsolete
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, Serializer);
#pragma warning restore CS0618 // Type or member is obsolete
        // ReSharper disable once AccessToDisposedClosure
        Assert.ThrowsExactly<System.Exception>(() => messagesToHtmlWriter.Dispose());
        byte[] before = bytes.ToArray();
        try { messagesToHtmlWriter.Dispose(); }
        catch (System.Exception e)
        {
            throw new System.Exception($"Expected no exception, but got {e.Message}");
        }
        byte[] after = bytes.ToArray();
        CollectionAssert.AreEqual(before, after);
    }

    [TestMethod]
    public async Task ItIsIdempotentUnderFailureToCloseAsync()
    {
        var bytes = new StreamThatFailsToClose();
        MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, AsyncSerializer);
        await Assert.ThrowsExactlyAsync<System.Exception>(async () => await messagesToHtmlWriter.DisposeAsync());
        byte[] before = bytes.ToArray();
        try { await messagesToHtmlWriter.DisposeAsync(); }
        catch (System.Exception e)
        {
            throw new System.Exception($"Expected no exception, but got {e.Message}");
        }
        byte[] after = bytes.ToArray();
        CollectionAssert.AreEqual(before, after);
    }

    [TestMethod]
    public async Task ItWritesTwoMessagesSeparatedByACommaAsync()
    {
        Envelope testRunStarted = Envelope.Create(new TestRunStarted(Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null));
        Envelope envelope = Envelope.Create(new TestRunFinished(null, true, Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(15).ToUniversalTime()), null, null));
        string html = await RenderAsHtmlAsync(testRunStarted, envelope);
        StringAssert.Contains(html, "window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}},{\"testRunFinished\":{\"success\":true,\"timestamp\":{\"seconds\":15,\"nanos\":0}}}];");
    }

    [TestMethod]
    public async Task ItEscapesOpeningAngleBracketAsync()
    {
        Envelope envelope = Envelope.Create(new GherkinDocument(
            null,
            null,
            [new(new Location(0L, 0L), "</script><script>alert('Hello')</script>")]
        ));
        string html = await RenderAsHtmlAsync(envelope);
        StringAssert.Contains(html, "window.CUCUMBER_MESSAGES = [{\"gherkinDocument\":{\"comments\":[{\"location\":{\"line\":0,\"column\":0},\"text\":\"\\x3C/script>\\x3Cscript>alert('Hello')\\x3C/script>\"}]}}];"}]}}];",
            "Expected \"window.CUCUMBER_MESSAGES = [{{\\\"gherkinDocument\\\":{{\\\"comments\\\":[{{\\\"location\\\":{{\\\"line\\\":0,\\\"column\\\":0}},\\\"text\\\":\\\"\\\\x3C/script>\\\\x3Cscript>alert('Hello')\\\\x3C/script>\\\"}}]}}];" +
                        $"\nbut instead had: \n{html.Substring(html.IndexOf("window.CUCUMBER"))}");
    }

    private static string RenderAsHtml(params Envelope[] messages)
    {
        MemoryStream bytes = new MemoryStream();
#pragma warning disable CS0618 // Type or member is obsolete
        using (MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, Serializer))
#pragma warning restore CS0618 // Type or member is obsolete
        {
            foreach (Envelope message in messages)
            {
                messagesToHtmlWriter.Write(message);
            }
        }
        return UTF8.GetString(bytes.ToArray());
    }

    private static async Task<string> RenderAsHtmlAsync(params Envelope[] messages)
    {
        return await RenderAsHtmlAsync(null, messages);
    }

    private static async Task<string> RenderAsHtmlAsync(HtmlReportSettings? settings, params Envelope[] messages)
    {
        MemoryStream bytes = new MemoryStream();
        await using (MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, AsyncSerializer, settings))
        {
            foreach (Envelope message in messages)
            {
                await messagesToHtmlWriter.WriteAsync(message);
            }
            await messagesToHtmlWriter.DisposeAsync();
        }
        return UTF8.GetString(bytes.ToArray());
    }
}