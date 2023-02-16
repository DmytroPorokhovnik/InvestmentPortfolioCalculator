using System.Reflection;
using PortfolioCalculator.Common;
using PortfolioCalculator.Interfaces;

namespace PortfolioCalculator
{
  public class Program
  {
    /// <summary>
    /// Calculates portfolio by input
    /// </summary>
    /// <param name="args">Could be empty or 3 file paths for investments, transactions and quotes only in this order</param>
    public static async Task Main(string[] args)
    {
      var reader = GetPortfolioReader(args);
      var investmentsTask = reader.GetInvestmentsAsync(CancellationToken.None);
      var quotesTask = reader.GetQuotesAsync(CancellationToken.None);
      var transactionsTask = reader.GetTransactionsAsync(CancellationToken.None);

      await Task.WhenAll(investmentsTask, quotesTask, transactionsTask);
    }

    private static IPortfolioReader GetPortfolioReader(IReadOnlyList<string> args)
    {
      var workPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if (workPath == null)
      {
        throw new InvalidOperationException("Null work path");
      }

      var importFilesDirectory = Path.Combine(workPath, Constants.ImportFilesDefaultFolderName);
      return args.Count == 3
        ? new CsvPortfolioReader(args[0], args[1], args[2])
        : new CsvPortfolioReader(Path.Combine(importFilesDirectory, Constants.InvestmentsFileName),
          Path.Combine(importFilesDirectory, Constants.TransactionsFileName), Path.Combine(importFilesDirectory, Constants.QuotesFileName));
    }
  }
}
