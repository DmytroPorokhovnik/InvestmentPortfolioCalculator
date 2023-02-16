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

    private readonly ICollection<Investment> _investments;
    private readonly ICollection<Transaction> _transactions;
    private readonly ICollection<Quote> _quotes;

    #endregion

    #region Public Properties

    

    #endregion

    #region Constructor

    /// <summary>
    /// Create a new portfolio calculator using set of all available investments, transactions and quotes
    /// </summary>
    /// <exception cref="ArgumentNullException">Throws if an argument is null</exception>
    public Calculator(ICollection<Investment> investments, ICollection<Transaction> transactions, ICollection<Quote> quotes)
    {
      _investments = investments ?? throw new ArgumentNullException(nameof(investments));
      _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
      _quotes = quotes ?? throw new ArgumentNullException(nameof(quotes));
    }

    #endregion

  }
}
