using System.Collections.Generic;

namespace Abstractions
{
    /// <summary>
    /// Interface represents contracts for creation <see cref="IReader"/> for different cases
    /// </summary>
    public interface IReaderFactory
    {
        /// <summary>
        /// Method creates single data reader for particular file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Reader for read file</returns>
        IReader CreateSingleReader(string fileName);

        /// <summary>
        /// Method creates collection of readers with partition underlying file per reader
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="numLines">Number of lines one reader can read</param>
        /// <returns>Collection of readers</returns>
        IReadOnlyCollection<IReader> CreateReaderPerLines(string fileName, int numLines);
    }
}