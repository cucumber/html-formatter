﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Text.Encoding;
using Io.Cucumber.Messages.Types;
using Cucumber.Messages;
using Cucumber.HtmlFormatter;

namespace Cucumber.HtmlFormatterTest
{
    [TestClass]
    public class MessagesToHtmlWriterTest
    {
        static readonly Action<StreamWriter, Envelope> serializer = new Action<StreamWriter, Envelope>((sw, e) =>
        {
            var s = NdjsonSerializer.Serialize(e);
            sw.Write(s);
        });

        static readonly Func<StreamWriter, Envelope, Task> asyncSerializer = async (sw, e) =>
        {
            var s = NdjsonSerializer.Serialize(e);
            await sw.WriteAsync(s);
        };


        [TestMethod]
        public void ItWritesOneMessageToHtml()
        {
            DateTime timestamp = DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime();
            Envelope envelope = Envelope.Create(new TestRunStarted(Converters.ToTimestamp(timestamp), null));
            string html = RenderAsHtml(envelope);
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}}];"),
                $"Expected html to contain a testRunStarted message, but instead html contained: {html}");
        }

        [TestMethod]
        public async Task ItWritesOneMessageToHtmlAsync()
        {
            DateTime timestamp = DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime();
            Envelope envelope = Envelope.Create(new TestRunStarted(Converters.ToTimestamp(timestamp), null));
            string html = await RenderAsHtmlAsync(envelope);
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}}];"),
                $"Expected html to contain a testRunStarted message, but instead html contained: {html}");
        }

        [TestMethod]
        public void ItWritesNoMessageToHtml()
        {
            string html = RenderAsHtml();
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [];"));
        }

        [TestMethod]
        public async Task ItWritesNoMessageToHtmlAsync()
        {
            string html = await RenderAsHtmlAsync();
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [];"));
        }

        [TestMethod]
        public void ItThrowsWhenWritingAfterClose()
        {
            MemoryStream bytes = new MemoryStream();
#pragma warning disable CS0618 // Type or member is obsolete
            MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer);
#pragma warning restore CS0618 // Type or member is obsolete
            messagesToHtmlWriter.Dispose();
            Assert.ThrowsException<IOException>(() => messagesToHtmlWriter.Write(Envelope.Create(new TestRunStarted(Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null))));
        }

        [TestMethod]
        public async Task ItThrowsWhenWritingAfterCloseAsync()
        {
            MemoryStream bytes = new MemoryStream();
            MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, asyncSerializer);
            await messagesToHtmlWriter.DisposeAsync();
            await Assert.ThrowsExceptionAsync<IOException>(async () =>
                await messagesToHtmlWriter.WriteAsync(
                    Envelope.Create(new TestRunStarted(
                        Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null))));
        }

        [TestMethod]
        public void ItCanBeClosedTwice()
        {
            MemoryStream bytes = new MemoryStream();
#pragma warning disable CS0618 // Type or member is obsolete
            MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer);
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
            MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, asyncSerializer);
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
            public override Task FlushAsync(System.Threading.CancellationToken cancellationToken)
            {
                throw new System.Exception("Can't flush this");
            }
        }
        [TestMethod]
        public void ItIsIdempotentUnderFailureToClose()
        {
            var bytes = new StreamThatFailsToClose();
#pragma warning disable CS0618 // Type or member is obsolete
            MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.ThrowsException<System.Exception>(() => messagesToHtmlWriter.Dispose());
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
            MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, asyncSerializer);
            await Assert.ThrowsExceptionAsync<System.Exception>(async () => await messagesToHtmlWriter.DisposeAsync());
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
        public void ItWritesTwoMessagesSeparatedByAComma()
        {
            Envelope testRunStarted = Envelope.Create(new TestRunStarted(Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null));
            Envelope envelope = Envelope.Create(new TestRunFinished(null, true, Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(15).ToUniversalTime()), null, null));
            string html = RenderAsHtml(testRunStarted, envelope);
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}},{\"testRunFinished\":{\"success\":true,\"timestamp\":{\"seconds\":15,\"nanos\":0}}}];"));
        }

        [TestMethod]
        public async Task ItWritesTwoMessagesSeparatedByACommaAsync()
        {
            Envelope testRunStarted = Envelope.Create(new TestRunStarted(Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(10).ToUniversalTime()), null));
            Envelope envelope = Envelope.Create(new TestRunFinished(null, true, Converters.ToTimestamp(DateTime.UnixEpoch.AddSeconds(15).ToUniversalTime()), null, null));
            string html = await RenderAsHtmlAsync(testRunStarted, envelope);
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [{\"testRunStarted\":{\"timestamp\":{\"seconds\":10,\"nanos\":0}}},{\"testRunFinished\":{\"success\":true,\"timestamp\":{\"seconds\":15,\"nanos\":0}}}];"));
        }

        [TestMethod]
        public void ItEscapesForwardSlashes()
        {
            Envelope envelope = Envelope.Create(new GherkinDocument(
                null,
                null,
                new List<Comment> { new Comment(new Location(0L, 0L), "</script><script>alert('Hello')</script>") }
            ));
            string html = RenderAsHtml(envelope);
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [{\"gherkinDocument\":{\"comments\":[{\"location\":{\"line\":0,\"column\":0},\"text\":\"<\\/script><script>alert('Hello')<\\/script>\"}]}}];"),
                $"Expected \"window.CUCUMBER_MESSAGES = [{{\\\"gherkinDocument\\\":{{\\\"comments\\\":[{{\\\"location\\\":{{\\\"line\\\":0,\\\"column\\\":0}},\\\"text\\\":\\\"<\\\\/script><script>alert('Hello')<\\\\/script>\\\"}}]}}];" +
                $"\nbut instead had: \n{html.Substring(html.IndexOf("window.CUCUMBER"))}");
        }


        [TestMethod]
        public async Task ItEscapesForwardSlashesAsync()
        {
            Envelope envelope = Envelope.Create(new GherkinDocument(
                null,
                null,
                new List<Comment> { new Comment(new Location(0L, 0L), "</script><script>alert('Hello')</script>") }
            ));
            string html = await RenderAsHtmlAsync(envelope);
            Assert.IsTrue(html.Contains("window.CUCUMBER_MESSAGES = [{\"gherkinDocument\":{\"comments\":[{\"location\":{\"line\":0,\"column\":0},\"text\":\"<\\/script><script>alert('Hello')<\\/script>\"}]}}];"),
                $"Expected \"window.CUCUMBER_MESSAGES = [{{\\\"gherkinDocument\\\":{{\\\"comments\\\":[{{\\\"location\\\":{{\\\"line\\\":0,\\\"column\\\":0}},\\\"text\\\":\\\"<\\\\/script><script>alert('Hello')<\\\\/script>\\\"}}]}}];" +
                $"\nbut instead had: \n{html.Substring(html.IndexOf("window.CUCUMBER"))}");
        }

        private static string RenderAsHtml(params Envelope[] messages)
        {
            MemoryStream bytes = new MemoryStream();
#pragma warning disable CS0618 // Type or member is obsolete
            using (MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, serializer))
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
            MemoryStream bytes = new MemoryStream();
            using (MessagesToHtmlWriter messagesToHtmlWriter = new MessagesToHtmlWriter(bytes, asyncSerializer))
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
}
