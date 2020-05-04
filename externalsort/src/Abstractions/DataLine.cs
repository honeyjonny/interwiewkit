using System;

namespace Abstractions
{
    /// <summary>
    /// Class represents a line of data received from data source
    /// </summary>
    public sealed class DataLine
    {
        public int Number { get; }

        public string StringData { get; }

        public DataLine(int number, string stringData)
        {
            Number = number;
            StringData = stringData;
        }
    }
}
