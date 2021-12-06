using System;
using System.IO;
using System.Threading.Tasks;

namespace create_token_test_environment
{
    /// <summary>
    /// Orchestrates the writing of the scenario’s distribution CSV file 
    /// for ingestion by the distribution tool.
    /// </summary>
    public class DistributionFileWriter : IDisposable
    {
        /// <summary>
        /// Output file’s stream writer.
        /// </summary>
        private StreamWriter _stream;
        /// <summary>
        /// Wrapper around the Stream Writer to format output 
        /// as valid CSV information.
        /// </summary>
        private CsvWriter _writer;
        /// <summary>
        /// Opens the distribution file for writing (overwrites pre-existing files) 
        /// and pre-populates with header information, return a reference to the 
        /// file writer for further additions of recipient secrets information.
        /// </summary>
        /// <param name="filePath">
        /// Path to the distribution file to create.  If a file exists at this path 
        /// it will be deleted prior to creating the new file.
        /// </param>
        /// <returns>
        /// A distribution file writer object that can be used to add additional 
        /// recipient information to the distribution file.
        /// </returns>
        public static async Task<DistributionFileWriter> OpenDistributionFile(string filePath)
        {
            var distributionWriter = new DistributionFileWriter(filePath);
            await distributionWriter.WriteHeaderInfo();
            return distributionWriter;
        }
        /// <summary>
        /// Private constructor that sets up the internal state of the writer.  
        /// Since writing the header is an async function, it is necessary to 
        /// create the OpenDistributionFile method to combine the construction 
        /// of the object paired with writing the initial header  information.
        /// </summary>
        /// <param name="path">
        /// Full pathname of the distribution csv file to create.
        /// </param>
        private DistributionFileWriter(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _stream = File.CreateText(path);
            _writer = new CsvWriter(_stream);
        }
        /// <summary>
        /// Incrementally writes the details of a newly created distribution 
        /// recipient and amount to the output file.
        /// </summary>
        /// <param name="acountInfo">
        /// The account information for the newly created distribution account, 
        /// includes the ID of the account.
        /// </param>
        /// <param name="amount">
        /// The amount of distribution to assign to this account (in whole token 
        /// amounts, not the smallest denomination amount).
        /// </param>
        public async Task WriteRecipientInfo(AccountInfo acountInfo, double amount)
        {
            var address = acountInfo.Receipt.Address.AsString();
            await _writer.WriteLineAsync(address, amount.ToString());
            await _stream.FlushAsync();
        }
        /// <summary>
        /// Writes the distribution csv file’s header information.
        /// </summary>
        private async Task WriteHeaderInfo()
        {
            await _writer.WriteLineAsync("# Test Distributions");
            await _stream.FlushAsync();
        }
        /// <summary>
        /// Dispose function that closes the output file and 
        /// releases system resources.
        /// </summary>
        public void Dispose()
        {
            _stream?.Flush();
            _stream?.Dispose();
            _writer = null;
            _stream = null;
        }
    }
}
