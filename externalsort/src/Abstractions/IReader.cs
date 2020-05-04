using System.Threading;
using System.Threading.Tasks;

namespace Abstractions
{
    /// <summary>
    /// Interface represents read abstraction over data source
    /// </summary>
    public interface IReader
    {
        /// <summary>
        /// Method gets next data line
        /// </summary>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Task representing data line or null if data ends</returns>
        Task<DataLine> GetNextLine(CancellationToken cancellation);
    }
}