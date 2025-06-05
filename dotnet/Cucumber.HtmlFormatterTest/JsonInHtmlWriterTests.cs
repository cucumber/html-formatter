using Cucumber.HtmlFormatter;
using System.Runtime.CompilerServices;
using System.Text;


namespace Cucumber.HtmlFormatterTest
{
    [TestClass]
    public sealed class JsonInHtmlWriterTests
    {
        private readonly MemoryStream outStream = new MemoryStream();
        private readonly StreamWriter outputStreamWriter;
        private readonly JsonInHtmlWriter writer;

        public JsonInHtmlWriterTests()
        {
            outputStreamWriter = new StreamWriter(outStream, new UTF8Encoding(false));
            writer = new JsonInHtmlWriter(outputStreamWriter);
        }

        [TestMethod]
        public void Writes()
        {
            writer.Write("{\"hello\": \"world\"}");
            Assert.AreEqual("{\"hello\": \"world\"}", Output());
        }

        [TestMethod]
        public async Task WritesAsync()
        {
            await writer.WriteAsync("{\"hello\": \"world\"}");
            Assert.AreEqual("{\"hello\": \"world\"}", await OutputAsync());
        }

        [TestMethod]
        public void EscapesSingle()
        {
            writer.Write("/");
            Assert.AreEqual("\\/", Output());
        }

        [TestMethod]
        public async Task EscapesSingleAsync()
        {
            await writer.WriteAsync("/");
            Assert.AreEqual("\\/", await OutputAsync());
        }

        [TestMethod]
        public void EscapesMultiple()
        {
            writer.Write("</script><script></script>");
            Assert.AreEqual("<\\/script><script><\\/script>", Output());
        }

        [TestMethod]
        public async Task EscapesMultipleAsync()
        {
            await writer.WriteAsync("</script><script></script>");
            Assert.AreEqual("<\\/script><script><\\/script>", await OutputAsync());
        }

        [TestMethod]
        public void PartialWrites()
        {
            char[] buffer = new char[100];
            string text = "</script><script></script>";

            text.CopyTo(0, buffer, 0, 9);
            writer.Write(buffer, 0, 9);

            text.CopyTo(9, buffer, 2, 8);
            writer.Write(buffer, 2, 8);

            text.CopyTo(17, buffer, 4, 9);
            writer.Write(buffer, 4, 9);

            Assert.AreEqual("<\\/script><script><\\/script>", Output());
        }

        [TestMethod]
        public async Task PartialWritesAsync()
        {
            char[] buffer = new char[100];
            string text = "</script><script></script>";

            text.CopyTo(0, buffer, 0, 9);
            await writer.WriteAsync(buffer, 0, 9);

            text.CopyTo(9, buffer, 2, 8);
            await writer.WriteAsync(buffer, 2, 8);

            text.CopyTo(17, buffer, 4, 9);
            await writer.WriteAsync(buffer, 4, 9);

            Assert.AreEqual("<\\/script><script><\\/script>", await OutputAsync());
        }
        [TestMethod]
        public void LargeWritesWithOddBoundaries()
        {
            char[] buffer = new char[1024];
            buffer[0] = 'a';
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] = '/';
            }
            writer.Write(buffer);

            StringBuilder expected = new StringBuilder();
            expected.Append('a');
            for (int i = 1; i < buffer.Length; i++)
            {
                expected.Append("\\/");
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
                buffer[i] = '/';
            }
            await writer.WriteAsync(buffer);

            StringBuilder expected = new StringBuilder();
            expected.Append('a');
            for (int i = 1; i < buffer.Length; i++)
            {
                expected.Append("\\/");
            }
            Assert.AreEqual(expected.ToString(), await OutputAsync());
        }

        [TestMethod]
        public void ReallyLargeWrites()
        {
            char[] buffer = new char[2048];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = '/';
            }
            writer.Write(buffer);

            StringBuilder expected = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                expected.Append("\\/");
            }
            Assert.AreEqual(expected.ToString(), Output());
        }

        [TestMethod]
        public async Task ReallyLargeWritesAsync()
        {
            char[] buffer = new char[2048];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = '/';
            }
            await writer.WriteAsync(buffer);

            StringBuilder expected = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                expected.Append("\\/");
            }
            Assert.AreEqual(expected.ToString(), await OutputAsync());
        }

        [TestMethod]
        public void EmptyWrite()
        {
            char[] buffer = new char[0];
            writer.Write(buffer);
            Assert.AreEqual("", Output());
        }

        [TestMethod]
        public async Task EmptyWriteAsync()
        {
            char[] buffer = new char[0];
            await writer.WriteAsync(buffer);
            Assert.AreEqual("", await OutputAsync());
        }

        private string Output()
        {
            writer.Flush();
            return Encoding.UTF8.GetString(outStream.ToArray());
        }

        private async Task<string> OutputAsync()
        {
            await writer.FlushAsync();
            return Encoding.UTF8.GetString(outStream.ToArray());
        }

    }
}
