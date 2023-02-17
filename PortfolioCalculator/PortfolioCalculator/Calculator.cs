using System.Collections.Concurrent;
using PortfolioCalculator.Interfaces;
using PortfolioCalculator.Models;

namespace PortfolioCalculator
{
  /// <summary>
  /// Portfolio calculator
  /// </summary>
  internal class Calculator : IPortfolioCalculator
  {
    #region Fields

    private readonly ConcurrentDictionary<string, List<Investment>> _investmentsDictionary;
    private readonly ConcurrentDictionary<string, List<Transaction>> _transactionDictionary;
    private readonly ConcurrentDictionary<string, SortedSet<Quote>> _sortedQuotesDictionary;

    #endregion

    #region Constructor

    /// <summary>
    /// Create a new portfolio calculator using set of all available investments, transactions and quotes
    /// </summary>
    /// <exception cref="ArgumentNullException">Throws if an argument is null</exception>
    public Calculator(IEnumerable<Investment> investments, IEnumerable<Transaction> transactions, IEnumerable<Quote> quotes)
    {
      if (investments == null)
      {
        throw new ArgumentNullException(nameof(investments));
      }

      if (transactions == null)
      {
        throw new ArgumentNullException(nameof(transactions));
      }

      if (quotes == null)
      {
        throw new ArgumentNullException(nameof(quotes));
      }

      _investmentsDictionary = new ConcurrentDictionary<string, List<Investment>>(investments.GroupBy(x => x.InvestorId).ToDictionary(x => x.Key, x => x.ToList()));
      _transactionDictionary = new ConcurrentDictionary<string, List<Transaction>>(transactions.GroupBy(t => t.InvestmentId).ToDictionary(x => x.Key, x => x.ToList()));
      
      var quoteDateComparer = Comparer<Quote>.Create((x, y) => x.Date < y.Date ? -1 : 1);
      _sortedQuotesDictionary = new ConcurrentDictionary<string, SortedSet<Quote>>(quotes.GroupBy(q => q.Id)
        .ToDictionary(x => x.Key, x => new SortedSet<Quote>(x.ToList(), quoteDateComparer)));
    }

    #endregion

    #region Public Methods

    /// <inheritdoc />
    public double CalculateInvestorPortfolioValue(string investorId, DateTime valueDate)
    {
      var sharesValue = CalculateSharesInvestmentsValue(investorId, valueDate);
      var fondsValue = CalculateFondInvestmentsValue(investorId, valueDate);
      var propertyValue = CalculatePropertyInvestmentsValue(investorId, valueDate);

      return sharesValue + fondsValue + propertyValue;
    }

    /// <inheritdoc />
    public double CalculateSharesInvestmentsValue(string investorId, DateTime valueDate)
    {
      var result = 0.0;
      if (!_investmentsDictionary.TryGetValue(investorId, out var investments))
      {
        return result;
      }

      foreach (var investment in investments.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount - 1)
                 .Where(i => i.InvestmentType == InvestmentType.Stock && !string.IsNullOrEmpty(i.ShareId)))
      {
        if (!_transactionDictionary.TryGetValue(investment.InvestmentId, out var transactions))
        {
          continue;
        }

        var shareTransactionsBeforeDate = transactions.Where(t => t.Date <= valueDate && t.Type == TransactionType.Shares);
        var sharesNumberOnDate = shareTransactionsBeforeDate.Sum(t => t.Value);
        result += GetShareValueOnDate(investment.ShareId, valueDate, sharesNumberOnDate);
      }

      return result;
    }

    /// <inheritdoc />
    public double CalculateFondInvestmentsValue(string investorId, DateTime valueDate)
    {
      if (!_investmentsDictionary.TryGetValue(investorId, out var investments))
      {
        return 0.0;
      }

      IEnumerable<string> investedFonds = investments.Where(i => i.InvestmentType == InvestmentType.Fond && !string.IsNullOrEmpty(i.FondInvestor))
        .Select(i => i.FondInvestor).Distinct()!;

      return (from fond in investedFonds.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount - 1)
        let fondValue = GetFondMarketValue(fond, valueDate)
        let fondOwnedPercentage = GetInvestorFondPercentage(investorId, fond, valueDate)
        select fondValue * fondOwnedPercentage / 100).Sum();
    }

    /// <inheritdoc />
    public double CalculatePropertyInvestmentsValue(string investorId, DateTime valueDate)
    {
      if (!_investmentsDictionary.TryGetValue(investorId, out var investments))
      {
        return 0.0;
      }

      var result = 0.0;
      foreach (var realEstateInvestments in investments.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount - 1)
                 .Where(i => i.InvestmentType == InvestmentType.RealEstate && !string.IsNullOrEmpty(i.City)))
      {
        if (!_transactionDictionary.TryGetValue(realEstateInvestments.InvestmentId, out var transactions))
        {
          continue;
        }

        var realEstateTransactionsBeforeDate = transactions.Where(t => t.Date <= valueDate && t.Type is TransactionType.Estate or TransactionType.Building);
        result += realEstateTransactionsBeforeDate.Sum(t => t.Value);
      }

      return result;
    }

    #endregion

    #region Private Methods

    private double GetShareValueOnDate(string? shareId, DateTime valueDate, double shareNumber)
    {
      if (string.IsNullOrEmpty(shareId))
      {
        return 0.0;
      }

      if (!_sortedQuotesDictionary.TryGetValue(shareId, out var quotes))
      {
        return 0.0;
      }

      var lastAvailableShareValue = quotes.LastOrDefault(q => q.Date <= valueDate)?.PricePerShare ?? 0.0;
      return lastAvailableShareValue * shareNumber;
    }

    private double GetFondMarketValue(string fondId, DateTime valueDate)
    {
      var fondSharesValue = CalculateSharesInvestmentsValue(fondId, valueDate);
      var fondRealEstateValue = CalculatePropertyInvestmentsValue(fondId, valueDate);
      return fondRealEstateValue + fondSharesValue;
    }

    private double GetInvestorFondPercentage(string investorId, string fondId, DateTime valueDate)
    {
      var result = 0.0;

      if (!_investmentsDictionary.TryGetValue(investorId, out var investments))
      {
        return result;
      }

      var fondInvestments = investments.Where(i => i.InvestmentType == InvestmentType.Fond && i.FondInvestor == fondId);

      foreach (var investment in fondInvestments.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount - 1).Where( i => !string.IsNullOrEmpty(i.FondInvestor)))
      {
        if (!_transactionDictionary.TryGetValue(investment.InvestmentId, out var transactions))
        {
          continue;
        }

        var fondTransactionsBeforeDate = transactions.Where(t => t.Date <= valueDate && t.Type == TransactionType.Percentage);
        result += fondTransactionsBeforeDate.Sum(t => t.Value);
      }

      return result;
    }

    #endregion

  }
}
