using PortfolioCalculator.Models;

namespace PortfolioCalculator.Interfaces
{
  /// <summary>
  /// Represents a reader for a portfolio that consists of investments, transactions and quotes
  /// </summary>
  internal interface IPortfolioReader
  {
    /// <summary>
    /// Gets all investments from a file asynchronously
    /// </summary>
    /// <param name="cancellationToken">token for cancellation</param>
    /// <returns>An <see cref="Task{IEnumerable{Investment>}}"/>  of result</returns>
    Task<IEnumerable<Investment>> GetInvestmentsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets all transactions from a file asynchronously
    /// </summary>
    /// <param name="cancellationToken">token for cancellation</param>
    /// <returns>An <see cref="Task{IEnumerable{Transaction>}}"/>  of result</returns>
    Task<IEnumerable<Transaction>> GetTransactionsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets all quotes from a file asynchronously
    /// </summary>
    /// <param name="cancellationToken">token for cancellation</param>
    /// <returns>An <see cref="Task{IEnumerable{Quote>}}"/>  of result</returns>
    Task<IEnumerable<Quote>> GetQuotesAsync(CancellationToken cancellationToken);
  }
}
