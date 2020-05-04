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
    }
}
