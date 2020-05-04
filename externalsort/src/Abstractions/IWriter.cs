using System.Threading;
using System.Threading.Tasks;

namespace Abstractions
{
    /// <summary>
    /// Interface represents abstraction over write operation
    /// </summary>
    public interface IWriter
    {
        /// <summary>
        /// Method writes piece of data
        /// </summary>
        /// <param name="dataLine">Data to write</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Task representing write operation</returns>
        Task WriteLine(DataLine dataLine, CancellationToken cancellation);
    }
}