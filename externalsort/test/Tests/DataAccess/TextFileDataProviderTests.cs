using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    /// <summary>
    /// Tests suite for <see cref="TextFileDataProvider"/>
    /// </summary>
    public sealed class TextFileDataProviderTests: TestBase
    {
        [Fact]
        public async Task ReadDataFromStream_Success()
        {
            var cts = new CancellationTokenSource();
            string source = "123. Apple";

            using (Stream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                var textProvider = ServiceProvider.GetRequiredService<TextFileDataProvider>();

                DataLine dataLine = await textProvider.GetNextLine(memoryStream, cts.Token);

                Assert.NotNull(dataLine);
                Assert.Equal(123, dataLine.Number);
                Assert.True(
                    StringComparer.InvariantCultureIgnoreCase.Equals("Apple", dataLine.StringData));
            }
        }

        [Fact]
        public async Task ReadDataFromStream_AsBulk_Success()
        {
            var cts = new CancellationTokenSource();
            string source = "123. Apple\n222. Banana";

            using (Stream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
            {
                var textProvider = ServiceProvider.GetRequiredService<TextFileDataProvider>();

                DataLine[] lines = await textProvider.GetLines(memoryStream, cts.Token);

                Assert.NotNull(lines);
                Assert.Equal(2, lines.Length);

                DataLine dataLine = lines[0];
                Assert.Equal(123, dataLine.Number);
                Assert.True(
                    StringComparer.InvariantCultureIgnoreCase.Equals("Apple", dataLine.StringData));

                DataLine dataLine1 = lines[1];
                Assert.Equal(222, dataLine1.Number);
                Assert.True(
                    StringComparer.InvariantCultureIgnoreCase.Equals("Banana", dataLine1.StringData));
            }
        }

        [Fact]
        public async Task WriteDataIntoStream_Success()
        {
            var cts = new CancellationTokenSource();
            DataLine line = new DataLine(123, "Apple");

            using (Stream stream = new MemoryStream())
            {
                var textProvider = ServiceProvider.GetRequiredService<TextFileDataProvider>();

                await textProvider.WriteLine(line, stream, cts.Token);

                stream.Seek(0, SeekOrigin.Begin);

                using (TextReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string stringLine = await reader.ReadLineAsync();

                    Assert.NotNull(stringLine);
                    Assert.True(
                        StringComparer.InvariantCultureIgnoreCase.Equals(
                            "123. Apple", stringLine));
                }
            }
        }

        [Fact]
        public async Task WriteDataIntoStream_AsBulk_Success()
        {
            var cts = new CancellationTokenSource();
            DataLine[] lines = new[]
            {
                new DataLine(123, "Apple"),
                new DataLine(222, "Banana"),
                new DataLine(555, "Banana"),
            };

            byte[] arr = new byte[1024];

            var textProvider = ServiceProvider.GetRequiredService<TextFileDataProvider>();

            using (Stream stream = new MemoryStream(arr))
            {
                await textProvider.WriteAll(lines, stream, cts.Token);
            }

            using (Stream stream = new MemoryStream(arr))
            {
                DataLine nextLine = await textProvider.GetNextLine(stream, cts.Token);

                Assert.NotNull(nextLine);
                Assert.Equal(123, nextLine.Number);
                Assert.Equal("Apple", nextLine.StringData);

                nextLine = await textProvider.GetNextLine(stream, cts.Token);

                Assert.NotNull(nextLine);
                Assert.Equal(222, nextLine.Number);
                Assert.Equal("Banana", nextLine.StringData);

                nextLine = await textProvider.GetNextLine(stream, cts.Token);

                Assert.NotNull(nextLine);
                Assert.Equal(555, nextLine.Number);
                Assert.Equal("Banana", nextLine.StringData);
            }
        }
    }
}
