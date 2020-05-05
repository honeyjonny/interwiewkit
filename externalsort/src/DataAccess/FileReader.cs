using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Abstractions;

namespace DataAccess
{
    /// <summary>
    /// Class represents implementation of <see cref="IReader"/> for reading files
    /// </summary>
    public sealed class FileReader: IReader
    {
        private readonly Stream _dataStream;
        private readonly IDataProvider _dataProvider;
        private bool _disposed = false;

        public FileReader(string filePath, IDataProvider dataProvider)
        {
            _dataStream = new FileStream(filePath, FileMode.Open);
            _dataProvider = dataProvider;
        }

        public FileReader(Stream dataStream, IDataProvider dataProvider)
        {
            _dataStream = dataStream;
            _dataProvider = dataProvider;
        }

        public Task<DataLine> GetNextLine(CancellationToken cancellation)
        {
            return _dataProvider.GetNextLine(_dataStream, cancellation);
        }

        public Task<DataLine[]> GetAllLines(CancellationToken cancellation)
        {
            return _dataProvider.GetLines(_dataStream, cancellation);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dataStream.Dispose();
                }
            }

            _disposed = true;
        }
    }
}