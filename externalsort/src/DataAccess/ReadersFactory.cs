using System;
using System.Collections.Generic;
using System.IO;
using Abstractions;

namespace DataAccess
{
    /// <summary>
    /// Class represents factory for creating readers
    /// </summary>
    public sealed class ReadersFactory: IReaderFactory
    {
        private readonly IDataProvider _dataProvider;

        public ReadersFactory(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public IReader CreateSingleReader(string filePath)
        {
            return new FileReader(filePath, _dataProvider);
        }

        public IEnumerable<IReader> CreateReaderPerLines(string fileName, int numLines)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                long position = 0L;
                bool end = false;
                do
                {
                    (long nextPosition, MemoryStream readedStream, bool eof) =
                        GetLinesNumberExactly(stream, position, numLines);

                    position = nextPosition;
                    end = eof == true;

                    yield return new FileReader(readedStream, _dataProvider);

                } while (!end);
            }
        }

        private (long nextPosition, MemoryStream readedStream, bool eof) GetLinesNumberExactly(
            Stream stream, 
            long startPosition,
            int linesNumber)
        {
            int lineCount = 0;

            Span<byte> buffer = stackalloc byte[1024 * 1024];
            MemoryStream outStream = new MemoryStream(buffer.Length);

            const byte lineEnd = (byte)'\n';

            int bytesRead = 0;

            long lastUnreadedPosition = startPosition;
            stream.Position = startPosition;

            bool eof = true;

            while ((bytesRead = stream.Read(buffer)) > 0)
            {
                for (var i = 0; i < bytesRead; i++)
                {
                    byte currentChar = buffer[i];
                    lastUnreadedPosition++;

                    if (currentChar == lineEnd)
                    {
                        lineCount++;
                    }

                    if (lineCount == linesNumber)
                    {
                        Span<byte> slice = buffer.Slice(0, i);
                        outStream.Write(slice);
                        eof = false;
                        break;
                    }
                }

                Span<byte> scannedSlice = buffer.Slice(0, bytesRead);
                outStream.Write(scannedSlice);
            }

            return (lastUnreadedPosition, outStream, eof);
        }
    }
}