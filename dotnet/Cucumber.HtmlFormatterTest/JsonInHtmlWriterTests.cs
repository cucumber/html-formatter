using Cucumber.HtmlFormatter;
using System.Text;

namespace Cucumber.HtmlFormatterTest;

[TestClass]
public sealed class JsonInHtmlWriterTests
{
    private readonly MemoryStream _outStream = new();
    private readonly JsonInHtmlWriter _writer;

    public JsonInHtmlWriterTests()
    {
        var outputStreamWriter = new StreamWriter(_outStream, new UTF8Encoding(false));
        _writer = new JsonInHtmlWriter(outputStreamWriter);
    }

    [TestMethod]
    public void Writes()
    {
        _writer.Write("{\"hello\": \"world\"}");
        Assert.AreEqual("{\"hello\": \"world\"}", Output());
    }

    [TestMethod]
    public async Task WritesAsync()
    {
        await _writer.WriteAsync("{\"hello\": \"world\"}");
        Assert.AreEqual("{\"hello\": \"world\"}", await OutputAsync());
    }

    [TestMethod]
    public void EscapesSingle()
    {
        _writer.Write("<");
        Assert.AreEqual("\\x3C", Output());
    }

    [TestMethod]
    public async Task EscapesSingleAsync()
    {
        await _writer.WriteAsync("<");
        Assert.AreEqual("\\x3C", await OutputAsync());
    }

    [TestMethod]
    public void EscapesMultiple()
    {
        _writer.Write("</script><script></script>");
        Assert.AreEqual("\\x3C/script>\\x3Cscript>\\x3C/script>", Output());
    }

    [TestMethod]
    public async Task EscapesMultipleAsync()
    {
        await _writer.WriteAsync("</script><script></script>");
        Assert.AreEqual("\\x3C/script>\\x3Cscript>\\x3C/script>", await OutputAsync());
    }

    [TestMethod]
    public void PartialWrites()
    {
        char[] buffer = new char[100];
        string text = "</script><script></script>";

        text.CopyTo(0, buffer, 0, 9);
        _writer.Write(buffer, 0, 9);

        text.CopyTo(9, buffer, 2, 8);
        _writer.Write(buffer, 2, 8);

        text.CopyTo(17, buffer, 4, 9);
        _writer.Write(buffer, 4, 9);

        Assert.AreEqual("\\x3C/script>\\x3Cscript>\\x3C/script>", Output());
    }

    [TestMethod]
    public async Task PartialWritesAsync()
    {
        char[] buffer = new char[100];
        string text = "</script><script></script>";

        text.CopyTo(0, buffer, 0, 9);
        await _writer.WriteAsync(buffer, 0, 9);

        text.CopyTo(9, buffer, 2, 8);
        await _writer.WriteAsync(buffer, 2, 8);

        text.CopyTo(17, buffer, 4, 9);
        await _writer.WriteAsync(buffer, 4, 9);

        Assert.AreEqual("\\x3C/script>\\x3Cscript>\\x3C/script>", await OutputAsync());
    }
    [TestMethod]
    public void LargeWritesWithOddBoundaries()
    {
        char[] buffer = new char[1024];
        buffer[0] = 'a';
        for (int i = 1; i < buffer.Length; i++)
        {
            buffer[i] = '<';
        }
        _writer.Write(buffer);

        StringBuilder expected = new StringBuilder();
        expected.Append('a');
        for (int i = 1; i < buffer.Length; i++)
        {
            expected.Append("\\x3C");
        }
        Assert.AreEqual(expected.ToString(), Output());
    }

    [TestMethod]
    public async Task LargeWritesWithOddBoundariesAsync()
    {
        char[] buffer = new char[1024];
        buffer[0] = 'a';
        for (int i = 1; i < buffer.Length; i++)
        {
            buffer[i] = '<';
        }
        await _writer.WriteAsync(buffer);

        StringBuilder expected = new StringBuilder();
        expected.Append('a');
        for (int i = 1; i < buffer.Length; i++)
        {
            expected.Append("\\x3C");
        }
        Assert.AreEqual(expected.ToString(), await OutputAsync());
    }

    [TestMethod]
    public void ReallyLargeWrites()
    {
        char[] buffer = new char[2048];
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = '<';
        }
        _writer.Write(buffer);

        StringBuilder expected = new StringBuilder();
        for (int i = 0; i < buffer.Length; i++)
        {
            expected.Append("\\x3C");
        }
        Assert.AreEqual(expected.ToString(), Output());
    }

    [TestMethod]
    public async Task ReallyLargeWritesAsync()
    {
        char[] buffer = new char[2048];
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = '<';
        }
        await _writer.WriteAsync(buffer);

        StringBuilder expected = new StringBuilder();
        for (int i = 0; i < buffer.Length; i++)
        {
            expected.Append("\\x3C");
        }
        Assert.AreEqual(expected.ToString(), await OutputAsync());
    }

    [TestMethod]
    public void EmptyWrite()
    {
        char[] buffer = [];
        _writer.Write(buffer);
        Assert.AreEqual("", Output());
    }

    [TestMethod]
    public async Task EmptyWriteAsync()
    {
        char[] buffer = [];
        await _writer.WriteAsync(buffer);
        Assert.AreEqual("", await OutputAsync());
    }

    private string Output()
    {
        _writer.Flush();
        return Encoding.UTF8.GetString(_outStream.ToArray());
    }

    private async Task<string> OutputAsync()
    {
        await _writer.FlushAsync();
        return Encoding.UTF8.GetString(_outStream.ToArray());
    }

}