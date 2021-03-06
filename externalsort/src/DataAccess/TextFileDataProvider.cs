﻿using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Linq;
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

        ///<remarks>
        /// Current implementation of this method could not work with stream implementation
        /// that doesn't support change position of cursor in the stream.
        /// Source of this problem became because StreamReader reads far more from stream than
        /// it needs for one line and next read reads from empty stream.
        /// </remarks>
        public async Task<DataLine> GetNextLine(Stream stream, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            using (StreamReader reader = CreateReader(stream))
            {
                long lastPosition = stream.Position;

                var line = await reader.ReadLineAsync();

                long newPosition = lastPosition + GetLenInUtf8PlusLf(line);
                stream.Position = newPosition;

                return ParseLine(line);
            }
        }

        public async Task<DataLine[]> GetLines(Stream stream, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            using (StreamReader reader = CreateReader(stream))
            {
                var data = await reader.ReadToEndAsync();
                var strings = data.Split(new []{'\n'}, StringSplitOptions.RemoveEmptyEntries);
                return strings.Select(ParseLine).ToArray();
            }
        }

        private StreamReader CreateReader(Stream stream) => 
            new StreamReader(stream, Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

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

            using (StreamWriter writer = CreateWriter(stream))
            {
                string stringLine = PrepareLine(line);
                await writer.WriteLineAsync(stringLine);
            }
        }

        public Task WriteAll(DataLine[] lines, Stream stream, CancellationToken cancellation)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
            {
                byte[] array = null;
                bool taken = false;
                try
                {
                    var arrayLength = lines
                        .Select(PrepareLine)
                        .Select(GetLenInUtf8PlusLf)
                        .Sum();

                    array = ArrayPool<byte>.Shared.Rent(arrayLength);
                    taken = true;

                    CopyTo(array, lines);

                    writer.Write(array, 0, arrayLength);
                    writer.Flush();
                }
                finally
                {
                    if (taken)
                    {
                        ArrayPool<byte>.Shared.Return(array);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void CopyTo(byte[] array, DataLine[] lines)
        {
            int lastLength = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = $"{PrepareLine(lines[i])}\n";
                byte[] lineBytes = Encoding.UTF8.GetBytes(line);
                Array.Copy(lineBytes, 0, array, lastLength, lineBytes.Length);

                lastLength += lineBytes.Length;
            }
        }

        private StreamWriter CreateWriter(Stream stream) =>
            new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true);

        private static int GetLenInUtf8PlusLf(string val) => Encoding.UTF8.GetBytes(val).Length + 1;

        private string PrepareLine(DataLine data) => $"{data.Number:D}. {data.StringData}";
    }
}