using CsvHelper.Configuration.Attributes;

namespace PortfolioCalculator.Models
{
  internal class Investment
  {
    [Name("InvestorId")]
    public string InvestorId { get; init; }
    public string InvestmentId { get; init; }
    public InvestmentType InvestmentType { get; init; }
    public string? City { get; init; }

    [Name("ISIN")]
    public string? ShareId { get; init; }

    [Name("FondsInvestor")]
    public string? FondInvestor { get; init; }
  }
}
