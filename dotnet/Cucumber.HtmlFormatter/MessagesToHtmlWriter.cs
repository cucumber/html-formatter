using Io.Cucumber.Messages.Types;
using System.IO;

namespace Cucumber.HtmlFormatter;

public class MessagesToHtmlWriter : IDisposable
{
    private StreamWriter writer;
    private Func<StreamWriter, Envelope, Task> asyncStreamSerializer;
    private Action<StreamWriter, Envelope> streamSerializer;
    private string template;
    private JsonInHtmlWriter JsonInHtmlWriter;
    private bool streamClosed = false;
    private bool preMessageWritten = false;
    private bool firstMessageWritten = false;
    private bool postMessageWritten = false;
    private bool isAsyncInitialized = false;

    public MessagesToHtmlWriter(Stream stream, Action<StreamWriter, Envelope> streamSerializer) : this(new StreamWriter(stream), streamSerializer)
    {
    }
    public MessagesToHtmlWriter(Stream stream, Func<StreamWriter, Envelope, Task> asyncStreamSerializer) : this(new StreamWriter(stream), asyncStreamSerializer) { }

    public MessagesToHtmlWriter(StreamWriter writer, Action<StreamWriter, Envelope> streamSerializer)
    {
        this.writer = writer;
        this.streamSerializer = streamSerializer;
        // Create async wrapper for sync serializer
        this.asyncStreamSerializer = (w, e) => {
            streamSerializer(w, e);
            return Task.CompletedTask;
        };
        template = GetResource("index.mustache.html");
        JsonInHtmlWriter = new JsonInHtmlWriter(writer);
        isAsyncInitialized = false;
    }
    public MessagesToHtmlWriter(StreamWriter writer, Func<StreamWriter, Envelope, Task> asyncStreamSerializer)
    {
        this.writer = writer;
        this.asyncStreamSerializer = asyncStreamSerializer;
        // Create sync wrapper for async serializer (will block)
        this.streamSerializer = (w, e) => asyncStreamSerializer(w, e).GetAwaiter().GetResult();
        template = GetResource("index.mustache.html");
        JsonInHtmlWriter = new JsonInHtmlWriter(writer);
        isAsyncInitialized = true;
    }

    private void WritePreMessage()
    {
        WriteTemplateBetween(writer, template, null, "{{css}}");
        WriteResource(writer, "main.css");
        WriteTemplateBetween(writer, template, "{{css}}", "{{messages}}");
    }

    private async Task WritePreMessageAsync()
    {
        await WriteTemplateBetweenAsync(writer, template, null, "{{css}}");
        await WriteResourceAsync(writer, "main.css");
        await WriteTemplateBetweenAsync(writer, template, "{{css}}", "{{messages}}");
    }

    private void WritePostMessage()
    {
        WriteTemplateBetween(writer, template, "{{messages}}", "{{script}}");
        WriteResource(writer, "main.js");
        WriteTemplateBetween(writer, template, "{{script}}", null);
    }

    private async Task WritePostMessageAsync()
    {
        await WriteTemplateBetweenAsync(writer, template, "{{messages}}", "{{script}}");
        await WriteResourceAsync(writer, "main.js");
        await WriteTemplateBetweenAsync(writer, template, "{{script}}", null);
    }

    public void Write(Envelope envelope)
    {
        if (isAsyncInitialized)
        {
            // Log a warning or use other diagnostics
            System.Diagnostics.Debug.WriteLine("Warning: Using synchronous Write when initialized with async serializer");
        }

        if (streamClosed) { throw new IOException("Stream closed"); }

        if (!preMessageWritten)
        {
            WritePreMessage();
            preMessageWritten = true;
            writer.Flush();
        }
        if (!firstMessageWritten)
        {
            firstMessageWritten = true;
        }
        else
        {
            writer.Write(",");
            writer.Flush();
        }

        streamSerializer(JsonInHtmlWriter, envelope);
        JsonInHtmlWriter.Flush();
    }
    public async Task WriteAsync(Envelope envelope)
    {
        if (!isAsyncInitialized)
        {
            // Log a warning or use other diagnostics
            System.Diagnostics.Debug.WriteLine("Warning: Using asynchronous WriteAsync when initialized with sync serializer");
        }

        if (streamClosed) { throw new IOException("Stream closed"); }

        if (!preMessageWritten)
        {
            await WritePreMessageAsync();
            preMessageWritten = true;
            await writer.FlushAsync();
        }
        if (!firstMessageWritten)
        {
            firstMessageWritten = true;
        }
        else
        {
            await writer.WriteAsync(",");
            await writer.FlushAsync();
        }

        // Use the synchronous serializer in an async context
        await asyncStreamSerializer(JsonInHtmlWriter, envelope);
        await JsonInHtmlWriter.FlushAsync();
    }

    public void Dispose()
    {
        if (streamClosed) { return; }

        if (!preMessageWritten)
        {
            WritePreMessage();
            preMessageWritten = true;
        }
        if (!postMessageWritten)
        {
            WritePostMessage();
            postMessageWritten = true;
        }
        try
        {
            writer.Flush();
            writer.Close();
        }
        finally
        {
            streamClosed = true;
        }
    }

    public async Task DisposeAsync()
    {
        if (streamClosed) { return; }

        if (!preMessageWritten)
        {
            await WritePreMessageAsync();
            preMessageWritten = true;
        }
        if (!postMessageWritten)
        {
            await WritePostMessageAsync();
            postMessageWritten = true;
        }
        try
        {
            await writer.FlushAsync();
            writer.Close();
        }
        finally
        {
            streamClosed = true;
        }
    }

    private void WriteResource(StreamWriter writer, string v)
    {
        var resource = GetResource(v);
        writer.Write(resource);
    }

    private async Task WriteResourceAsync(StreamWriter writer, string v)
    {
        var resource = GetResource(v);
        await writer.WriteAsync(resource);
    }
    private void WriteTemplateBetween(StreamWriter writer, string template, string? begin, string? end)
    {
        int beginIndex, lengthToWrite;
        CalculateBeginAndLength(template, begin, end, out beginIndex, out lengthToWrite);
        writer.Write(template.Substring(beginIndex, lengthToWrite));
    }

    private static void CalculateBeginAndLength(string template, string? begin, string? end, out int beginIndex, out int lengthToWrite)
    {
        beginIndex = begin == null ? 0 : template.IndexOf(begin) + begin.Length;
        int endIndex = end == null ? template.Length : template.IndexOf(end);
        lengthToWrite = endIndex - beginIndex;
    }

    private async Task WriteTemplateBetweenAsync(StreamWriter writer, string template, string? begin, string? end)
    {
        int beginIndex, lengthToWrite;
        CalculateBeginAndLength(template, begin, end, out beginIndex, out lengthToWrite);
        await writer.WriteAsync(template.Substring(beginIndex, lengthToWrite));
    }
    private string GetResource(string name)
    {
        var assembly = typeof(MessagesToHtmlWriter).Assembly;
        var resourceStream = assembly.GetManifestResourceStream("Cucumber.HtmlFormatter.Resources." + name);
        var resource = new StreamReader(resourceStream).ReadToEnd();
        return resource;
    }
}
