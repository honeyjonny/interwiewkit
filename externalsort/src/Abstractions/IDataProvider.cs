using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Abstractions
{
    /// <summary>
    /// Interface represents abstraction over format of underlying data source
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Method for consume data from data source
        /// </summary>
        /// <param name="stream">Stream to consume data</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Task with next data line inside or null if no data available</returns>
        Task<DataLine> GetNextLine(Stream stream, CancellationToken cancellation);

        /// <summary>
        /// Method for consume data from source as bulk operation
        /// </summary>
        /// <param name="stream">Stream to consume data</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Task with array of data lines available</returns>
        Task<DataLine[]> GetLines(Stream stream, CancellationToken cancellation);

        /// <summary>
        /// Method for write data to data source
        /// </summary>
        /// <param name="line">Data line to write</param>
        /// <param name="stream">Stream to write data</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Task representing write operation</returns>
        Task WriteLine(DataLine line, Stream stream, CancellationToken cancellation);

        /// <summary>
        /// Method for write data to source as bulk operation
        /// </summary>
        /// <param name="lines">Array of lines to write</param>
        /// <param name="stream">Stream to write data</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Task representing write operation</returns>
        Task WriteAll(DataLine[] lines, Stream stream, CancellationToken cancellation);
    }
}