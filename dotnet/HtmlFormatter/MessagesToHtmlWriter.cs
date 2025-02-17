using Io.Cucumber.Messages.Types;
using System;
using System.IO;
namespace HtmlFormatter;

public class MessagesToHtmlWriter : IDisposable
{
    private StreamWriter writer;
    private Action<StreamWriter, Envelope> streamSerializer;
    private string template;
    private JsonInHtmlWriter JsonInHtmlWriter;
    private bool streamClosed = false;
    private bool preMessageWritten = false;
    private bool firstMessageWritten = false;
    private bool postMessageWritten = false;

    public MessagesToHtmlWriter(Stream stream, Action<StreamWriter, Envelope> streamSerializer) : this(new StreamWriter(stream), streamSerializer)
    {
    }
    public MessagesToHtmlWriter(StreamWriter writer, Action<StreamWriter, Envelope> streamSerializer)
    {
        this.writer = writer;
        this.streamSerializer = streamSerializer;
        template = GetResource("index.mustache.html");
        this.JsonInHtmlWriter = new JsonInHtmlWriter(writer);
    }

    private void writePreMessage()
    {
        writeTemplateBetween(writer, template, null, "{{css}}");
        writeResource(writer, "main.css");
        writeTemplateBetween(writer, template, "{{css}}", "{{messages}}");
    }

    private void writePostMessage()
    {
        writeTemplateBetween(writer, template, "{{messages}}", "{{script}}");
        writeResource(writer, "main.js");
        writeTemplateBetween(writer, template, "{{script}}", null);
    }

    public void Write(Envelope envelope)
    {
        if (streamClosed) { throw new System.Exception("Stream closed"); }

        if (!preMessageWritten)
        {
            writePreMessage();
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
    public void Dispose()
    {
        if (streamClosed) { return; }

        if (!preMessageWritten)
        {
            writePreMessage();
            preMessageWritten = true;
        }
        if (!postMessageWritten)
        {
            writePostMessage();
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
    private void writeResource(StreamWriter writer, string v)
    {
        var resource = GetResource(v);
        writer.Write(resource);
    }

    private void writeTemplateBetween(StreamWriter writer, string template, string begin, string end)
    {
        int beginIndex = begin == null ? 0 : template.IndexOf(begin) + begin.Length;
        int endIndex = end == null ? template.Length : template.IndexOf(end);
        int lengthToWrite = endIndex - beginIndex;
        writer.Write(template.Substring(beginIndex, lengthToWrite));
    }

    private string GetResource(string name)
    {
        var assembly = typeof(MessagesToHtmlWriter).Assembly;
        var resourceStream = assembly.GetManifestResourceStream("Cucumber.HtmlFormatter.Resources." + name);
        var resource = new StreamReader(resourceStream).ReadToEnd();
        return resource;
    }


}
