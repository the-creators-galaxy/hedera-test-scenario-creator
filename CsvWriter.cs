using System;
using System.IO;
using System.Threading.Tasks;

namespace create_token_test_environment
{
    /// <summary>
    /// A CSV Stream Formatter that manages the details of formatting data for CSV output.
    /// </summary>
    public class CsvWriter
    {
        /// <summary>
        /// A standard IO stream writer to send the formatted output to.
        /// </summary>
        private StreamWriter _destination;
        /// <summary>
        /// Constructor, requires a stream writer to write output to.
        /// </summary>
        /// <param name="destination">
        /// The destination stream writer receiving the formatted output.
        /// </param>
        public CsvWriter(StreamWriter destination)
        {
            _destination = destination;
        }
        /// <summary>
        /// Writes a CSV formatted line to the output stream, encodes strings 
        /// as necessary if they contain commas or quotes.
        /// </summary>
        /// <param name="data">
        /// Array of strings to send to the output stream as CSV formatted data.
        /// </param>
        public async Task WriteLineAsync(params string[] data)
        {
            if (data != null && data.Length > 0)
            {
                await WriteToken(data[0]);
                for (int index = 1; index < data.Length; index++)
                {
                    await _destination.WriteAsync(',');
                    await WriteToken(data[index]);
                }
            }
            await _destination.WriteLineAsync();
        }
        /// <summary>
        /// Internal helper function that examines a single output token to 
        /// determine if it needs special encoding because it contains special characters.
        /// </summary>
        /// <param name="data">
        /// The token to format.
        /// </param>
        private async Task WriteToken(String data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (data.Contains('"'))
                {
                    data = data.Replace("\"", "\"\"");
                    await _destination.WriteAsync('"');
                    await _destination.WriteAsync(data);
                    await _destination.WriteAsync('"');
                }
                else if (data.Contains(','))
                {
                    await _destination.WriteAsync('"');
                    await _destination.WriteAsync(data);
                    await _destination.WriteAsync('"');
                }
                else
                {
                    await _destination.WriteAsync(data);
                }
            }
        }
    }
}
