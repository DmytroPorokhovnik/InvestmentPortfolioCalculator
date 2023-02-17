using System.Diagnostics;
using System.Reflection;
using PortfolioCalculator.Common;
using PortfolioCalculator.Interfaces;
using PortfolioCalculator.Models;

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

      Console.WriteLine("Please enter investor id and date");
      var input = Console.ReadLine();
      if (string.IsNullOrWhiteSpace(input))
      {
        return;
      }

      await Task.WhenAll(investmentsTask, quotesTask, transactionsTask);
      IPortfolioCalculator calculator = new Calculator(investmentsTask.Result, transactionsTask.Result, quotesTask.Result);
      RunWithUserInput(input, calculator);
    }

    private static void RunWithUserInput(string? input, IPortfolioCalculator calculator)
    {
      while (!string.IsNullOrWhiteSpace(input))
      {
        try
        {
          var inputSplit = input.Split(";");
          if (inputSplit.Length != 2)
          {
            Console.WriteLine("Wrong input");
            continue;
          }

          if (!DateTime.TryParse(inputSplit[0], out var date))
          {
            Console.WriteLine("Date couldn't be parsed");
            continue;
          }

          var investorId = inputSplit[1];
          if (string.IsNullOrWhiteSpace(investorId))
          {
            Console.WriteLine("Investor id is empty");
            continue;
          }

          var portfolioValue = calculator.CalculateInvestorPortfolioValue(investorId, date);
          Console.WriteLine($"{investorId} portfolio values is {string.Format("{0:N}", portfolioValue)}");
        }
        finally
        {
          input = Console.ReadLine();
        }
      }
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
