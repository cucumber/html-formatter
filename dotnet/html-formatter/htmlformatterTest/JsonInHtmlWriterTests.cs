using HtmlFormatter;
using System.Text;


namespace htmlformatterTest
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
        public void EscapesSingle()
        {
            writer.Write("/");
            Assert.AreEqual("\\/", Output());
        }

        [TestMethod]
        public void EscapesMultiple()
        {
            writer.Write("</script><script></script>");
            Assert.AreEqual("<\\/script><script><\\/script>", Output());
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
        public void EmptyWrite()
        {
            char[] buffer = new char[0];
            writer.Write(buffer);
            Assert.AreEqual("", Output());
        }

        private string Output()
        {
            writer.Flush();
            return Encoding.UTF8.GetString(outStream.ToArray());
        }
    }
}
