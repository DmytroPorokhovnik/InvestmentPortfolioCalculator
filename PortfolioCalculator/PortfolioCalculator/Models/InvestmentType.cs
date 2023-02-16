using CsvHelper.Configuration.Attributes;

namespace PortfolioCalculator.Models
{
  internal enum InvestmentType
  {
    [Name("Fonds")]
    Fond,
    Stock,
    RealEstate
  }
}
