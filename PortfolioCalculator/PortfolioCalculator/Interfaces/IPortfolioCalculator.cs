namespace PortfolioCalculator.Interfaces
{
  /// <summary>
  /// Represent a portfolio calculator
  /// </summary>
  internal interface IPortfolioCalculator
  {
    /// <summary>
    /// Calculates whole portfolio value for the specified investor and date
    /// </summary>
    /// <param name="investorId">Investor, owner of portfolio id</param>
    /// <param name="valueDate">Desired date for portfolio value</param>
    /// <returns>Portfolio value</returns>
    double CalculateInvestorPortfolioValue(string investorId, DateTime valueDate);

    /// <summary>
    ///  Calculates portfolio shares value for the specified investor and date
    /// </summary>
    /// <param name="investorId">Investor, owner of portfolio id</param>
    /// <param name="valueDate">Desired date for portfolio value</param>
    /// <returns>Shares value</returns>
    double CalculateSharesInvestmentsValue(string investorId, DateTime valueDate);

    /// <summary>
    /// Calculates portfolio fond investments value for the specified investor and date
    /// </summary>
    /// <param name="investorId">Investor, owner of portfolio id</param>
    /// <param name="valueDate">Desired date for portfolio value</param>
    /// <returns>Value of investments into fonds</returns>
    double CalculateFondInvestmentsValue(string investorId, DateTime valueDate);

    /// <summary>
    /// Calculates portfolio property value for specified investor and date
    /// </summary>
    /// <param name="investorId">Investor, owner of portfolio id</param>
    /// <param name="valueDate">Desired date for portfolio value</param>
    /// <returns>Property investments value</returns>
    double CalculatePropertyInvestmentsValue(string investorId, DateTime valueDate);
  }
}
