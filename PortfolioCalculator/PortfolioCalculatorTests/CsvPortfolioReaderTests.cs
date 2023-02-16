using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using PortfolioCalculator;
using MissingFieldException = CsvHelper.MissingFieldException;

namespace PortfolioCalculatorTests
{
  [TestClass]
  public class CsvPortfolioReaderTests
  {
    private readonly string _testDataDirectory;
    private const string _csvTestDataFolderName = "CsvTestData";
    private const string _investmentsFileName = "Investments.csv";
    private const string _transactionsFileName = "Transactions.csv";
    private const string _quotesFileName = "Quotes.csv";
    private const string _quotesBigFileName = "QuotesBig.csv";
    private const string _xmlFile = "xmlFile.xml";

    public CsvPortfolioReaderTests()
    {
      _testDataDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), _csvTestDataFolderName);
    }

    #region CsvPortfolioReader Constructor Tests

    [TestMethod]
    public void CsvPortfolioReaderCtr_GivenValidPaths_CreatesReader()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));
    }

    [TestMethod]
    public void CsvPortfolioReaderCtr_GivenValidPathsAndCsvConfiguration_CreatesReader()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName), new CsvConfiguration(CultureInfo.InvariantCulture){Delimiter = ","});
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenEmptyInvestmentFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, ""), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenEmptyTransactionFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, ""),
        Path.Combine(_testDataDirectory, _quotesFileName));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenEmptyQuotesFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName), 
        Path.Combine(_testDataDirectory, ""));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenNonExistentInvestmentFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, "nonexistent.csv"), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenNonExistentTransactionFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, "nonexistent.csv"),
        Path.Combine(_testDataDirectory, _quotesFileName));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenNonExistentQuotesFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, "nonexistent.csv"));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenWrongExtensionInvestmentFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _xmlFile), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenWrongExtensionTransactionFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _xmlFile),
        Path.Combine(_testDataDirectory, _quotesFileName));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvPortfolioReaderCtr_GivenWrongExtensionQuotesFileName_ThrowsArgumentException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _xmlFile));
    }

    #endregion

    #region CsvPortfolioReader Reading Tests

    [TestMethod]
    public async Task GetQuotesAsync_GivenFileWith100Quotes_ReturnsQuotes()
    {
      var expectedEntriesNumber = 100;

      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));
      var quotes = await reader.GetQuotesAsync(CancellationToken.None);

      Assert.AreEqual(expectedEntriesNumber, quotes.Count());
    }

    [TestMethod]
    public async Task GetQuotesAsync_GivenFileWith50Investments_ReturnsQuotes()
    {
      var expectedEntriesNumber = 50;

      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));
      var investments = await reader.GetInvestmentsAsync(CancellationToken.None);

      Assert.AreEqual(expectedEntriesNumber, investments.Count());
    }

    [TestMethod]
    public async Task GetQuotesAsync_GivenFileWith100Transactions_ReturnsQuotes()
    {
      var expectedEntriesNumber = 100;

      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));
      var transactions = await reader.GetTransactionsAsync(CancellationToken.None);

      Assert.AreEqual(expectedEntriesNumber, transactions.Count());
    }

    [TestMethod]
    [ExpectedException(typeof(HeaderValidationException))]
    public async Task GetQuotesAsync_GivenWrongCsvDelimiter_ThrowsHeaderValidationException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName), new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ","});
      await reader.GetQuotesAsync(CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(MissingFieldException))]
    public async Task GetQuotesAsync_GivenWrongCsvDelimiterWithoutHeaderValidation_ThrowsMissingFieldException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName),
        Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName), new CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = ",", HeaderValidated = null});
      await reader.GetQuotesAsync(CancellationToken.None);
    }

    [TestMethod]
    [ExpectedException(typeof(OperationCanceledException))]
    public async Task GetQuotesAsync_GivenCancelledToken_ThrowsOperationCancelledException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName),
        Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesFileName));
      var tokenSource = new CancellationTokenSource();
      tokenSource.Cancel();
      await reader.GetQuotesAsync(tokenSource.Token);
    }

    [TestMethod]
    [ExpectedException(typeof(OperationCanceledException))]
    public async Task GetQuotesAsync_GivenCancelledTokenAfter_ThrowsOperationCancelledException()
    {
      var reader = new CsvPortfolioReader(Path.Combine(_testDataDirectory, _investmentsFileName), Path.Combine(_testDataDirectory, _transactionsFileName),
        Path.Combine(_testDataDirectory, _quotesBigFileName));
      var tokenSource = new CancellationTokenSource();
      tokenSource.CancelAfter(100);

      await reader.GetQuotesAsync(tokenSource.Token);
    }

    #endregion
  }
}