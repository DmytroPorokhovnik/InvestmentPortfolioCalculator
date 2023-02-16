using CsvHelper.Configuration.Attributes;

namespace PortfolioCalculator.Models
{
  internal class Transaction
  {
    public string InvestmentId { get; init; }
    public TransactionType Type { get; init; }
    public DateTime Date { get; init; }
    public double Value { get; init; }
  }
}
