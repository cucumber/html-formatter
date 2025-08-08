using Io.Cucumber.Messages.Types;

namespace Cucumber.HtmlFormatter;

public class MessagesToHtmlWriter : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly Func<StreamWriter, Envelope, Task> _asyncStreamSerializer;
    private readonly Action<StreamWriter, Envelope> _streamSerializer;
    private readonly string _template;
    private readonly JsonInHtmlWriter _jsonInHtmlWriter;
    private readonly HtmlReportSettings _settings;
    private bool _streamClosed = false;
    private bool _preMessageWritten = false;
    private bool _firstMessageWritten = false;
    private bool _postMessageWritten = false;
    private readonly bool _isAsyncInitialized = false;
    
    // Define the points where we split the template for pre/post message
    private const string CSS_MARKER = "{{css}}";
    private const string MESSAGES_MARKER = "{{messages}}";
    private const string SCRIPT_MARKER = "{{script}}";
    private const string TITLE_MARKER = "{{title}}";
    private const string ICON_MARKER = "{{icon}}";
    private const string CUSTOM_CSS_MARKER = "{{custom-css}}";
    private const string CUSTOM_HEAD_MARKER = "{{custom-head}}";

    [Obsolete("Cucumber.HtmlFormatter moving to async only operations. Please use the MessagesToHtmlWriter(Stream, Func<StreamWriter, Envelope, Task>) constructor", false)]
    public MessagesToHtmlWriter(Stream stream, Action<StreamWriter, Envelope> streamSerializer) 
        : this(new StreamWriter(stream), streamSerializer)
    {
    }

    [Obsolete("Cucumber.HtmlFormatter moving to async only operations. Please use the MessagesToHtmlWriter(Stream, Func<StreamWriter, Envelope, Task>, HtmlReportSettings) constructor", false)]
    public MessagesToHtmlWriter(Stream stream, Action<StreamWriter, Envelope> streamSerializer, HtmlReportSettings? settings) 
        : this(new StreamWriter(stream), streamSerializer, settings)
    {
    }

    public MessagesToHtmlWriter(Stream stream, Func<StreamWriter, Envelope, Task> asyncStreamSerializer) 
        : this(new StreamWriter(stream), asyncStreamSerializer)
    {
    }

    public MessagesToHtmlWriter(Stream stream, Func<StreamWriter, Envelope, Task> asyncStreamSerializer, 
        HtmlReportSettings? settings = null) 
        : this(new StreamWriter(stream), asyncStreamSerializer, settings)
    {
    }

    [Obsolete("Cucumber.HtmlFormatter moving to async only operations. Please use the MessagesToHtmlWriter(StreamWriter, Func<StreamWriter, Envelope, Task>) constructor", false)]
    public MessagesToHtmlWriter(StreamWriter writer, Action<StreamWriter, Envelope> streamSerializer)
        : this(writer, streamSerializer, null)
    {
    }

    [Obsolete("Cucumber.HtmlFormatter moving to async only operations. Please use the MessagesToHtmlWriter(StreamWriter, Func<StreamWriter, Envelope, Task>, HtmlReportSettings) constructor", false)]
    public MessagesToHtmlWriter(StreamWriter writer, Action<StreamWriter, Envelope> streamSerializer, HtmlReportSettings? settings)
    {
        this._writer = writer;
        this._streamSerializer = streamSerializer;
        // Create async wrapper for sync serializer
        this._asyncStreamSerializer = (w, e) =>
        {
            streamSerializer(w, e);
            return Task.CompletedTask;
        };
        _settings = settings ?? new HtmlReportSettings();
        _template = GetResource("index.mustache.html");
        _jsonInHtmlWriter = new JsonInHtmlWriter(writer);
        _isAsyncInitialized = false;
    }

    public MessagesToHtmlWriter(StreamWriter writer, Func<StreamWriter, Envelope, Task> asyncStreamSerializer)
        : this(writer, asyncStreamSerializer, null)
    {
    }

    public MessagesToHtmlWriter(StreamWriter writer, Func<StreamWriter, Envelope, Task> asyncStreamSerializer,
        HtmlReportSettings? settings = null)
    {
        this._writer = writer;
        this._asyncStreamSerializer = asyncStreamSerializer;
        // Create sync wrapper for async serializer (will block)
        this._streamSerializer = (w, e) => asyncStreamSerializer(w, e).GetAwaiter().GetResult();
        this._settings = settings ?? new HtmlReportSettings();
        _template = GetResource("index.mustache.html");
        _jsonInHtmlWriter = new JsonInHtmlWriter(writer);
        _isAsyncInitialized = true;
    }

    private void WritePreMessage()
    {
        // Process template from beginning to CSS marker
        string processedTemplate = ApplySettingsToTemplate(_template);

        // For backward compatibility, maintain the same order of template processing
        WriteTemplateBetween(_writer, processedTemplate, null, CSS_MARKER);
        WriteResource(_writer, "main.css");
        WriteTemplateBetween(_writer, processedTemplate, CSS_MARKER, MESSAGES_MARKER);
    }

    private async Task WritePreMessageAsync()
    {
        // Process template from beginning to CSS marker
        string processedTemplate = ApplySettingsToTemplate(_template);

        // For backward compatibility, maintain the same order of template processing
        await WriteTemplateBetweenAsync(_writer, processedTemplate, null, CSS_MARKER);
        await WriteResourceAsync(_writer, "main.css");
        await WriteTemplateBetweenAsync(_writer, processedTemplate, CSS_MARKER, MESSAGES_MARKER);
    }

    private void WritePostMessage()
    {
        // Process template from messages to end
        string processedTemplate = ApplySettingsToTemplate(_template);

        // For backward compatibility, maintain the same order of template processing
        WriteTemplateBetween(_writer, processedTemplate, MESSAGES_MARKER, SCRIPT_MARKER);
        WriteResource(_writer, "main.js");
        WriteTemplateBetween(_writer, processedTemplate, SCRIPT_MARKER, null);
    }

    private async Task WritePostMessageAsync()
    {
        // Process template from messages to end
        string processedTemplate = ApplySettingsToTemplate(_template);

        // For backward compatibility, maintain the same order of template processing
        await WriteTemplateBetweenAsync(_writer, processedTemplate, MESSAGES_MARKER, SCRIPT_MARKER);
        await WriteResourceAsync(_writer, "main.js");
        await WriteTemplateBetweenAsync(_writer, processedTemplate, SCRIPT_MARKER, null);
    }

    private string ApplySettingsToTemplate(string template)
    {
        // Apply all custom settings to the template
        var result = template;
        
        // Apply title placeholder if it exists in the template
        if (template.Contains(TITLE_MARKER))
        {
            result = result.Replace(TITLE_MARKER, _settings.Title);
        }
        
        // Apply icon placeholder if it exists in the template
        if (template.Contains(ICON_MARKER))
        {
            result = result.Replace(ICON_MARKER, _settings.Icon);
        }
        
        // Apply custom CSS placeholder if it exists in the template
        if (template.Contains(CUSTOM_CSS_MARKER))
        {
            result = result.Replace(CUSTOM_CSS_MARKER, _settings.CustomCss);
        }
        
        // Apply custom head placeholder if it exists in the template
        if (template.Contains(CUSTOM_HEAD_MARKER))
        {
            result = result.Replace(CUSTOM_HEAD_MARKER, _settings.CustomHead);
        }
        
        return result;
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
        string section = template.Substring(beginIndex, lengthToWrite);
        writer.Write(section);
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
        string section = template.Substring(beginIndex, lengthToWrite);
        await writer.WriteAsync(section);
    }

    private string GetResource(string name)
    {
        var assembly = typeof(MessagesToHtmlWriter).Assembly;
        var resourceStream = assembly.GetManifestResourceStream("Cucumber.HtmlFormatter.Resources." + name);
        if (resourceStream == null)
            throw new InvalidOperationException($"Resource '{name}' not found in assembly '{assembly.FullName}'");
        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }
}
