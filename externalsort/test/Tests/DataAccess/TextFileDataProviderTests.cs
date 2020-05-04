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

        private async Task WriteToStream(Stream stream, string str)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await writer.WriteLineAsync(str);
            }
        }
    }
}
