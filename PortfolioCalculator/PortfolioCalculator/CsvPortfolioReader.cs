using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using PortfolioCalculator.Common;
using PortfolioCalculator.Interfaces;
using PortfolioCalculator.Models;

namespace PortfolioCalculator
{
  /// <summary>
  /// CSV file reader for Portfolios.
  /// Portfolios info is separated into 3 files: investments, transactions, quotes
  /// </summary>
  internal class CsvPortfolioReader : IPortfolioReader
  {
    #region Fields

    private readonly string _investmentsFilePath;
    private readonly string _transactionsFilePath;
    private readonly string _quotesFilePath;
    private readonly IReaderConfiguration _csvConfiguration;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new CsvPortfolioReader using Paths to the needed portfolios info and csv delimiter
    /// </summary>
    /// <exception cref="ArgumentException">Throws if path is invalid or a file doesn't exist</exception>
    public CsvPortfolioReader(string investmentsFilePath, string transactionsFilePath, string quotesFilePath, IReaderConfiguration? csvConfiguration = null)
    {
      if (string.IsNullOrEmpty(investmentsFilePath) || Path.GetExtension(investmentsFilePath) != Constants.CsvExtension ||
          !File.Exists(investmentsFilePath))
      {
        throw new ArgumentException(nameof(investmentsFilePath));
      }

      if (string.IsNullOrEmpty(transactionsFilePath) || Path.GetExtension(transactionsFilePath) != Constants.CsvExtension ||
          !File.Exists(transactionsFilePath))
      {
        throw new ArgumentException(nameof(transactionsFilePath));
      }

      if (string.IsNullOrEmpty(quotesFilePath) || Path.GetExtension(quotesFilePath) != Constants.CsvExtension ||
          !File.Exists(quotesFilePath))
      {
        throw new ArgumentException(nameof(quotesFilePath));
      }

      _investmentsFilePath = investmentsFilePath;
      _transactionsFilePath = transactionsFilePath;
      _quotesFilePath = quotesFilePath;
      _csvConfiguration = csvConfiguration ?? new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = Constants.CsvDelimiter};
    }

    #endregion


    #region Public Methods

    /// <inheritdoc />
    public async Task<IEnumerable<Investment>> GetInvestmentsAsync(CancellationToken cancellationToken)
    {
      using var stream = new StreamReader(_investmentsFilePath);
      using var reader = new CsvReader(stream, _csvConfiguration);
      return await reader.GetRecordsAsync<Investment>(cancellationToken).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Transaction>> GetTransactionsAsync(CancellationToken cancellationToken)
    {
      using var stream = new StreamReader(_transactionsFilePath);
      using var reader = new CsvReader(stream, _csvConfiguration);
      var recordsAsyncEnumerable = reader.GetRecordsAsync<Transaction>(cancellationToken);
      return await recordsAsyncEnumerable.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Quote>> GetQuotesAsync(CancellationToken cancellationToken)
    {
      using var stream = new StreamReader(_quotesFilePath);
      using var reader = new CsvReader(stream, _csvConfiguration);
      return await reader.GetRecordsAsync<Quote>(cancellationToken).ToListAsync(cancellationToken);
    }

    #endregion

  }
}
