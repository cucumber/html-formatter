using Io.Cucumber.Messages.Types;

namespace Cucumber.HtmlFormatter;

public class MessagesToHtmlWriter : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly Func<StreamWriter, Envelope, Task> _asyncStreamSerializer;
    private readonly Action<StreamWriter, Envelope> _streamSerializer;
    private readonly string _template;
    private readonly JsonInHtmlWriter _jsonInHtmlWriter;
    private bool _streamClosed = false;
    private bool _preMessageWritten = false;
    private bool _firstMessageWritten = false;
    private bool _postMessageWritten = false;
    private readonly bool _isAsyncInitialized = false;

    [Obsolete("Cucumber.HtmlFormatter moving to async only operations. Please use the MessagesToHtmlWriter(Stream, Func<StreamWriter, Envelope, Task>) constructor", false)]
    public MessagesToHtmlWriter(Stream stream, Action<StreamWriter, Envelope> streamSerializer) : this(new StreamWriter(stream), streamSerializer)
    {
    }
    public MessagesToHtmlWriter(Stream stream, Func<StreamWriter, Envelope, Task> asyncStreamSerializer) : this(new StreamWriter(stream), asyncStreamSerializer) { }

    [Obsolete("Cucumber.HtmlFormatter moving to async only operations. Please use the MessagesToHtmlWriter(StreamWriter, Func<StreamWriter, Envelope, Task>) constructor", false)]
    public MessagesToHtmlWriter(StreamWriter writer, Action<StreamWriter, Envelope> streamSerializer)
    {
        this._writer = writer;
        this._streamSerializer = streamSerializer;
        // Create async wrapper for sync serializer
        this._asyncStreamSerializer = (w, e) =>
        {
            streamSerializer(w, e);
            return Task.CompletedTask;
        };
        _template = GetResource("index.mustache.html");
        _jsonInHtmlWriter = new JsonInHtmlWriter(writer);
        _isAsyncInitialized = false;
    }
    public MessagesToHtmlWriter(StreamWriter writer, Func<StreamWriter, Envelope, Task> asyncStreamSerializer)
    {
        this._writer = writer;
        this._asyncStreamSerializer = asyncStreamSerializer;
        // Create sync wrapper for async serializer (will block)
        this._streamSerializer = (w, e) => asyncStreamSerializer(w, e).GetAwaiter().GetResult();
        _template = GetResource("index.mustache.html");
        _jsonInHtmlWriter = new JsonInHtmlWriter(writer);
        _isAsyncInitialized = true;
    }

    private void WritePreMessage()
    {
        WriteTemplateBetween(_writer, _template, null, "{{css}}");
        WriteResource(_writer, "main.css");
        WriteTemplateBetween(_writer, _template, "{{css}}", "{{messages}}");
    }

    private async Task WritePreMessageAsync()
    {
        await WriteTemplateBetweenAsync(_writer, _template, null, "{{css}}");
        await WriteResourceAsync(_writer, "main.css");
        await WriteTemplateBetweenAsync(_writer, _template, "{{css}}", "{{messages}}");
    }

    private void WritePostMessage()
    {
        WriteTemplateBetween(_writer, _template, "{{messages}}", "{{script}}");
        WriteResource(_writer, "main.js");
        WriteTemplateBetween(_writer, _template, "{{script}}", null);
    }

    private async Task WritePostMessageAsync()
    {
        await WriteTemplateBetweenAsync(_writer, _template, "{{messages}}", "{{script}}");
        await WriteResourceAsync(_writer, "main.js");
        await WriteTemplateBetweenAsync(_writer, _template, "{{script}}", null);
    }

    public void Write(Envelope envelope)
    {
        if (_isAsyncInitialized)
        {
            // Log a warning or use other diagnostics
            System.Diagnostics.Debug.WriteLine("Warning: Using synchronous Write when initialized with async serializer");
        }

        if (_streamClosed) { throw new IOException("Stream closed"); }

        if (!_preMessageWritten)
        {
            WritePreMessage();
            _preMessageWritten = true;
            _writer.Flush();
        }
        if (!_firstMessageWritten)
        {
            _firstMessageWritten = true;
        }
        else
        {
            _writer.Write(",");
            _writer.Flush();
        }

        _streamSerializer(_jsonInHtmlWriter, envelope);
        _jsonInHtmlWriter.Flush();
    }

    public async Task WriteAsync(Envelope envelope)
    {
        if (!_isAsyncInitialized)
        {
            // Log a warning or use other diagnostics
            System.Diagnostics.Debug.WriteLine("Warning: Using asynchronous WriteAsync when initialized with sync serializer");
        }

        if (_streamClosed) { throw new IOException("Stream closed"); }

        if (!_preMessageWritten)
        {
            await WritePreMessageAsync();
            _preMessageWritten = true;
            await _writer.FlushAsync();
        }
        if (!_firstMessageWritten)
        {
            _firstMessageWritten = true;
        }
        else
        {
            await _writer.WriteAsync(",");
            await _writer.FlushAsync();
        }

        // Use the synchronous serializer in an async context
        await _asyncStreamSerializer(_jsonInHtmlWriter, envelope);
        await _jsonInHtmlWriter.FlushAsync();
    }

    public void Dispose()
    {
        if (_streamClosed) { return; }

        if (!_preMessageWritten)
        {
            WritePreMessage();
            _preMessageWritten = true;
        }
        if (!_postMessageWritten)
        {
            WritePostMessage();
            _postMessageWritten = true;
        }
        try
        {
            _writer.Flush();
            _writer.Close();
        }
        finally
        {
            _streamClosed = true;
        }
    }

    public async Task DisposeAsync()
    {
        if (_streamClosed) { return; }

        if (!_preMessageWritten)
        {
            await WritePreMessageAsync();
            _preMessageWritten = true;
        }
        if (!_postMessageWritten)
        {
            await WritePostMessageAsync();
            _postMessageWritten = true;
        }
        try
        {
            await _writer.FlushAsync();
            _writer.Close();
        }
        finally
        {
            _streamClosed = true;
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
        CalculateBeginAndLength(template, begin, end, out var beginIndex, out var lengthToWrite);
        writer.Write(template.Substring(beginIndex, lengthToWrite));
    }

    private static void CalculateBeginAndLength(string template, string? begin, string? end, out int beginIndex, out int lengthToWrite)
    {
        beginIndex = begin == null ? 0 : template.IndexOf(begin, StringComparison.Ordinal) + begin.Length;
        int endIndex = end == null ? template.Length : template.IndexOf(end, StringComparison.Ordinal);
        lengthToWrite = endIndex - beginIndex;
    }

    private async Task WriteTemplateBetweenAsync(StreamWriter writer, string template, string? begin, string? end)
    {
        CalculateBeginAndLength(template, begin, end, out var beginIndex, out var lengthToWrite);
        await writer.WriteAsync(template.Substring(beginIndex, lengthToWrite));
    }

    private string GetResource(string name)
    {
        var assembly = typeof(MessagesToHtmlWriter).Assembly;
        var resourceStream = assembly.GetManifestResourceStream("Cucumber.HtmlFormatter.Resources." + name);
        if (resourceStream == null)
            throw new InvalidOperationException($"Resource '{name}' not found in assembly '{assembly.FullName}'");
        var resource = new StreamReader(resourceStream).ReadToEnd();
        return resource;
    }
}
