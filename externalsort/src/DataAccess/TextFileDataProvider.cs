using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;
using Microsoft.Extensions.Logging;

namespace DataAccess
{
    /// <summary>
    /// Class represents realization of <see cref="IDataProvider"/> for text files
    /// </summary>
    public sealed class TextFileDataProvider: IDataProvider
    {
        private readonly ILogger<TextFileDataProvider> _logger;

        public TextFileDataProvider(ILogger<TextFileDataProvider> logger)
        {
            _logger = logger;
        }

        public async Task<DataLine> GetNextLine(Stream stream, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, 
                detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true))
            {
                var line = await reader.ReadLineAsync();
                return ParseLine(line);
            }
        }

        private DataLine ParseLine(string line)
        {
            DataLine result = null;

            string[] strings = line.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length == 2)
            {
                string number = strings[0].Trim();
                if (int.TryParse(number, NumberStyles.None,
                    CultureInfo.InvariantCulture, out int intResult))
                {
                    string trimmedString = strings[1].Trim();
                    result = new DataLine(intResult, trimmedString);
                }
                else
                {
                    _logger.LogError(
                        $"Error with parsing string that expected to be number: {number}. " +
                        $"Parsing will return null as result of operation.");
                }
            }
            else
            {
                _logger.LogError(
                    $"Line of incoming text doesn't conform to expected format of text file. Line: {line}. " +
                    $"Parsing will return null as result of operation.");
            }

            return result;
        }

        public async Task WriteLine(DataLine line, Stream stream, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 
                bufferSize: 1024, leaveOpen: true))
            {
                string stringLine = PrepareLine(line);
                await writer.WriteLineAsync(stringLine);
            }
        }

        private string PrepareLine(DataLine data) => $"{data.Number}. {data.StringData}";
    }
}