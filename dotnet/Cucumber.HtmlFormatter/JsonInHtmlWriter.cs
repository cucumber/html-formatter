namespace Cucumber.HtmlFormatter;

public class JsonInHtmlWriter : StreamWriter
{
    private const int BUFFER_SIZE = 1024;
    private readonly StreamWriter _writer;
    private readonly char[] _escapeBuffer;

    public JsonInHtmlWriter(StreamWriter writer) : base(writer.BaseStream)
    {
        _writer = writer;
        _escapeBuffer = new char[BUFFER_SIZE]; // Initialize escapeBuffer
    }

    public override void Write(string value)
    {
        Write(value.ToCharArray(), 0, value.Length);
    }

    public override async Task WriteAsync(string value)
    {
        await WriteAsync(value.ToCharArray(), 0, value.Length);
    }

    public override void Write(char[] value)
    {
        Write(value, 0, value.GetLength(0));
    }

    public override void Write(char[] source, int offset, int length)
    {
        if (offset + length > source.GetLength(0))
            throw new ArgumentException("Cannot read past the end of the input source char array.");

        var destination = PrepareBuffer();
        // Largest write without boundary check is 4 bytes
        var flushAt = BUFFER_SIZE - 4;
        var written = 0;
        for (var i = offset; i < offset + length; i++)
        {
            var c = source[i];

            // Flush buffer if (nearly) full
            if (written >= flushAt)
            {
                _writer.Write(destination, 0, written);
                written = 0;
            }

            // Replace < with \x3C
            // https://html.spec.whatwg.org/multipage/scripting.html#restrictions-for-contents-of-script-elements
            if (c == '<')
            {
                destination[written++] = '\\';
                destination[written++] = 'x';
                destination[written++] = '3';
                destination[written++] = 'C';
            } 
            else
            {
                destination[written++] = c;
            }
        }
        // Flush any remaining
        if (written > 0)
        {
            _writer.Write(destination, 0, written);
        }
    }

    public override async Task WriteAsync(char[] source, int offset, int length)
    {
        if (offset + length > source.GetLength(0))
            throw new ArgumentException("Cannot read past the end of the input source char array.");

        var destination = PrepareBuffer();
        // Largest write without boundary check is 4 bytes
        var flushAt = BUFFER_SIZE - 4;
        var written = 0;
        for (var i = offset; i < offset + length; i++)
        {
            char c = source[i];

            // Flush buffer if (nearly) full
            if (written >= flushAt)
            {
                await _writer.WriteAsync(destination, 0, written);
                written = 0;
            }

            // Replace < with \x3C
            // https://html.spec.whatwg.org/multipage/scripting.html#restrictions-for-contents-of-script-elements
            if (c == '<')
            {
                destination[written++] = '\\';
                destination[written++] = 'x';
                destination[written++] = '3';
                destination[written++] = 'C';
            } 
            else
            {
                destination[written++] = c;
            }
        }
        // Flush any remaining
        if (written > 0)
        {
            await _writer.WriteAsync(destination, 0, written);
        }
    }

    private char[] PrepareBuffer()
    {
        // Reuse the same buffer, avoids repeated array allocation
        return _escapeBuffer;
    }

    public override void Flush()
    {
        _writer.Flush();
    }

    public override async Task FlushAsync()
    {
        await _writer.FlushAsync();
    }

    public override void Close()
    {
        _writer.Close();
    }
}
