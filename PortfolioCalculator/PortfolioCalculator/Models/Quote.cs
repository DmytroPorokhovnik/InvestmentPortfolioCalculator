using CsvHelper.Configuration.Attributes;

namespace PortfolioCalculator.Models
{
  internal class Quote
  {
    [Name("ISIN")]
    public string Id { get; init; }
    public DateTime Date { get; init; }
    public double PricePerShare { get; init; }
  }
}
