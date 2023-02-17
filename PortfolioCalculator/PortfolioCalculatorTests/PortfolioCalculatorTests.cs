using System.Reflection;
using PortfolioCalculator;
using PortfolioCalculator.Models;

namespace PortfolioCalculatorTests
{
  [TestClass]
  public class PortfolioCalculatorTests
  {
    #region Fields

    private readonly string _testDataDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), _calculatorDataFolderName);
    private const string _calculatorDataFolderName = "CalculatorTestData";
    private const string _investmentsFileName = "Investments.csv";
    private const string _transactionsFileName = "Transactions.csv";
    private const string _quotesFileName = "Quotes.csv";
    private const double _precision = 0.0001;
    private readonly (IEnumerable<Investment>, IEnumerable<Transaction>, IEnumerable<Quote>) _testData;

    #endregion

    public PortfolioCalculatorTests()
    {
      _testData = GetTestData();
    }

    #region PortfolioCalculator Contructor Tests

    [TestMethod]
    public void PortfolioCalculatorCtr_GivenPortfolioData_ReturnsCalculator()
    {
      var calculator = new Calculator(new List<Investment>(), new List<Transaction>(), new List<Quote>());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PortfolioCalculatorCtr_GivenNullInvestments_ThrowsArgumentNullException()
    {
      var calculator = new Calculator(null, new List<Transaction>(), new List<Quote>());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PortfolioCalculatorCtr_GivenNullTransactions_ThrowsArgumentNullException()
    {
      var calculator = new Calculator(new List<Investment>(), null, new List<Quote>());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PortfolioCalculatorCtr_GivenNullQuotes_ThrowsArgumentNullException()
    {
      var calculator = new Calculator(new List<Investment>(), new List<Transaction>(), null);
    }
    #endregion

    #region Calculate Investments Value Tests

    [TestMethod]
    public void CalculateSharesInvestmentsValue_GivenPortfolioData_ReturnSharesValue()
    {
      var calculator = new Calculator(_testData.Item1, _testData.Item2, _testData.Item3);

      var expectedValue = 4963.14702;
      var sharesValue = calculator.CalculateSharesInvestmentsValue("Investor0", DateTime.Today); // with last available price for all available transactions

      Assert.IsTrue(Math.Abs(expectedValue - sharesValue) < _precision);

      expectedValue = 3722.55418;
      sharesValue = calculator.CalculateSharesInvestmentsValue("Investor0", DateTime.Parse("2018-01-01")); // specific date

      Assert.IsTrue(Math.Abs(expectedValue - sharesValue) < _precision);

      expectedValue = 0.0;
      sharesValue = calculator.CalculateSharesInvestmentsValue("Investor1", DateTime.Today); // no data for shares

      Assert.AreEqual(expectedValue, sharesValue);

      expectedValue = 0.0;
      sharesValue = calculator.CalculateSharesInvestmentsValue("Investor2", DateTime.Today); // investor2 has no shares investments

      Assert.AreEqual(expectedValue, sharesValue);

      expectedValue = 0.0;
      sharesValue = calculator.CalculateSharesInvestmentsValue("NonExistentInvestor", DateTime.Today); // investor doesn't exist

      Assert.AreEqual(expectedValue, sharesValue);
    }

    [TestMethod]
    public void CalculatePropertyInvestmentsValue_GivenPortfolioData_ReturnPropertyValue()
    {
      var calculator = new Calculator(_testData.Item1, _testData.Item2, _testData.Item3);

      var expectedValue = 3422885.0;
      var propertyValue = calculator.CalculatePropertyInvestmentsValue("Investor0", DateTime.Today); // with last available price for all available transactions

      Assert.IsTrue(Math.Abs(expectedValue - propertyValue) < _precision);

      expectedValue = 2243420;
      propertyValue = calculator.CalculatePropertyInvestmentsValue("Fonds12", DateTime.Parse("2018-01-01")); // specific date

      Assert.IsTrue(Math.Abs(expectedValue - propertyValue) < _precision);

      expectedValue = 0.0;
      propertyValue = calculator.CalculatePropertyInvestmentsValue("Investor1", DateTime.Today); // investor1 has no shares investments

      Assert.AreEqual(expectedValue, propertyValue);

      expectedValue = 0.0;
      propertyValue = calculator.CalculatePropertyInvestmentsValue("NonExistentInvestor", DateTime.Today); // investor doesn't exist

      Assert.AreEqual(expectedValue, propertyValue);
    }

    [TestMethod]
    public void CalculateFondInvestmentsValue_GivenPortfolioData_ReturnFondInvestmentsValue()
    {
      var calculator = new Calculator(_testData.Item1, _testData.Item2, _testData.Item3);

      var expectedValue = 1741.59185;
      var fondInvestmentsValue = calculator.CalculateFondInvestmentsValue("Investor0", DateTime.Today); // with last available price for all available transactions

      Assert.IsTrue(Math.Abs(expectedValue - fondInvestmentsValue) < _precision);

      expectedValue = 1792.4516;
      fondInvestmentsValue = calculator.CalculateFondInvestmentsValue("Investor0", DateTime.Parse("2018-01-01")); // specific date

      Assert.IsTrue(Math.Abs(expectedValue - fondInvestmentsValue) < _precision);

      expectedValue = 0.0;
      fondInvestmentsValue = calculator.CalculateFondInvestmentsValue("Investor1", DateTime.Today); // investor1 has no shares investments

      Assert.AreEqual(expectedValue, fondInvestmentsValue);

      expectedValue = 0.0;
      fondInvestmentsValue = calculator.CalculateFondInvestmentsValue("NonExistentInvestor", DateTime.Today); // investor doesn't exist

      Assert.AreEqual(expectedValue, fondInvestmentsValue);
    }

    [TestMethod]
    public void CalculateInvestorPortfolioValue_GivenPortfolioData_ReturnFondInvestmentsValue()
    {
      var calculator = new Calculator(_testData.Item1, _testData.Item2, _testData.Item3);

      var expectedValue = 1741.59185 + 3422885.0 + 4963.14702;
      var portfolioValue = calculator.CalculateInvestorPortfolioValue("Investor0", DateTime.Today); // with last available price for all available transactions

      Assert.IsTrue(Math.Abs(expectedValue - portfolioValue) < _precision);

      expectedValue = 1133496.005842;
      portfolioValue = calculator.CalculateInvestorPortfolioValue("Investor0", DateTime.Parse("2018-01-01")); // specific date

      Assert.IsTrue(Math.Abs(expectedValue - portfolioValue) < _precision);

      expectedValue = 0.0;
      portfolioValue = calculator.CalculateInvestorPortfolioValue("Investor1", DateTime.Today); // investor1 has no investments

      Assert.AreEqual(expectedValue, portfolioValue);

      expectedValue = 0.0;
      portfolioValue = calculator.CalculateInvestorPortfolioValue("NonExistentInvestor", DateTime.Today); // investor doesn't exist

      Assert.AreEqual(expectedValue, portfolioValue);
    }

    #endregion

    #region Private Methods

    private (IEnumerable<Investment>, IEnumerable<Transaction>, IEnumerable<Quote>) GetTestData()
    {
      var investments = new List<Investment>()
      {
        new () {InvestorId = "Investor0", InvestmentId = "Investment44789", InvestmentType = InvestmentType.Fond, FondInvestor = "Fonds1"},
        new() {InvestorId = "Investor0", InvestmentId = "Investment48878", InvestmentType = InvestmentType.Fond, FondInvestor = "Fonds12"},

        new() {InvestorId = "Fonds1", InvestmentId = "Investment21900", InvestmentType = InvestmentType.Stock, ShareId = "ISIN62"},
        new() {InvestorId = "Fonds1", InvestmentId = "Investment29522", InvestmentType = InvestmentType.RealEstate, City = "City4522"},

        new() {InvestorId = "Fonds12", InvestmentId = "Investment12550", InvestmentType = InvestmentType.Stock, ShareId = "ISIN150"},
        new() {InvestorId = "Fonds12", InvestmentId = "Investment28930", InvestmentType = InvestmentType.RealEstate, City = "City3930"},

        new() {InvestorId = "Investor0", InvestmentId = "Investment5815", InvestmentType = InvestmentType.Stock, ShareId = "ISIN26"},
        new() {InvestorId = "Investor0", InvestmentId = "Investment12407", InvestmentType = InvestmentType.Stock, ShareId = "ISIN130"},
        new() {InvestorId = "Investor0", InvestmentId = "Investment22216", InvestmentType = InvestmentType.Stock, ShareId = "ISIN153"},

        new() {InvestorId = "Investor0", InvestmentId = "Investment29478", InvestmentType = InvestmentType.RealEstate, City = "City4478"},
        new() {InvestorId = "Investor0", InvestmentId = "Investment27611", InvestmentType = InvestmentType.RealEstate, City = "City2611"},
        new() {InvestorId = "Investor0", InvestmentId = "Investment29173", InvestmentType = InvestmentType.RealEstate, City = "City4173"},

        new() {InvestorId = "Investor1", InvestmentId = "Investment23835", InvestmentType = InvestmentType.Stock, FondInvestor = "ISIN117"},

        new() {InvestorId = "Investor2", InvestmentId = "Investment29481", InvestmentType = InvestmentType.RealEstate, City = "City4481"},
      };

      var quotes = new List<Quote>()
      {
        new () {Id = "ISIN26", Date = DateTime.Parse("2016-06-28"), PricePerShare = 98.2239},
        new() {Id = "ISIN26", Date = DateTime.Parse("2016-06-29"), PricePerShare = 98.1888},
        new() {Id = "ISIN26", Date = DateTime.Parse("2016-06-30"), PricePerShare = 98.642},
        new() {Id = "ISIN26", Date = DateTime.Parse("2016-07-01"), PricePerShare = 97.949},
        new() {Id = "ISIN26", Date = DateTime.Parse("2016-07-04"), PricePerShare = 98.1489},
        new() {Id = "ISIN26", Date = DateTime.Parse("2016-07-05"), PricePerShare = 99.5234},
        new() {Id = "ISIN26", Date = DateTime.Parse("2016-07-06"), PricePerShare = 99.5346},
        new() {Id = "ISIN26", Date = DateTime.Parse("2021-01-20"), PricePerShare = 104.8962},
        new() {Id = "ISIN26", Date = DateTime.Parse("2021-01-21"), PricePerShare = 105.6453},
        new() {Id = "ISIN26", Date = DateTime.Parse("2021-01-22"), PricePerShare = 105.1563},


        new() {Id = "ISIN130", Date = DateTime.Parse("2016-01-06"), PricePerShare = 6.16},
        new() {Id = "ISIN130", Date = DateTime.Parse("2016-01-07"), PricePerShare = 6.16},
        new() {Id = "ISIN130", Date = DateTime.Parse("2016-01-08"), PricePerShare = 6.17},
        new() {Id = "ISIN130", Date = DateTime.Parse("2016-01-11"), PricePerShare = 6.08},
        new() {Id = "ISIN130", Date = DateTime.Parse("2016-01-12"), PricePerShare = 6.11},
        new() {Id = "ISIN130", Date = DateTime.Parse("2021-07-19"), PricePerShare = 6.18},
        new() {Id = "ISIN130", Date = DateTime.Parse("2021-07-20"), PricePerShare = 17.57},
        new() {Id = "ISIN130", Date = DateTime.Parse("2021-01-21"), PricePerShare = 17.76},
        new() {Id = "ISIN130", Date = DateTime.Parse("2016-01-22"), PricePerShare = 17.75},
        new() {Id = "ISIN130", Date = DateTime.Parse("2016-01-25"), PricePerShare = 18.03},

        new() {Id = "ISIN150", Date = DateTime.Parse("2016-06-29"), PricePerShare = 104.31},
        new() {Id = "ISIN150", Date = DateTime.Parse("2016-06-30"), PricePerShare = 104.508},
        new() {Id = "ISIN150", Date = DateTime.Parse("2016-07-01"), PricePerShare = 105.088},
        new() {Id = "ISIN150", Date = DateTime.Parse("2019-10-24"), PricePerShare = 102.237},
        new() {Id = "ISIN150", Date = DateTime.Parse("2019-10-25"), PricePerShare = 102.034},

        new() {Id = "ISIN62", Date = DateTime.Parse("2016-06-28"), PricePerShare = 115.934},
        new() {Id = "ISIN62", Date = DateTime.Parse("2016-06-29"), PricePerShare = 115.926},
        new() {Id = "ISIN62", Date = DateTime.Parse("2016-06-30"), PricePerShare = 115.716},
        new() {Id = "ISIN62", Date = DateTime.Parse("2016-07-01"), PricePerShare = 115.943},
        new() {Id = "ISIN62", Date = DateTime.Parse("2016-07-04"), PricePerShare = 116.106},
        new() {Id = "ISIN62", Date = DateTime.Parse("2019-11-01"), PricePerShare = 125},
      };

      var transactions = new List<Transaction>
      {
        new () {InvestmentId = "Investment44789", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-01-03"), Value = 0.0403},
        new() {InvestmentId = "Investment44789", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-01-12"), Value = 0.008505},
        new() {InvestmentId = "Investment44789", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-01-15"), Value = -0.005453},
        new() {InvestmentId = "Investment44789", Type = TransactionType.Percentage, Date = DateTime.Parse("2018-02-22"), Value = 0.01368},
        new() {InvestmentId = "Investment44789", Type = TransactionType.Percentage, Date = DateTime.Parse("2018-08-11"), Value = -0.015834},

        new() {InvestmentId = "Investment48878", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-02-19"), Value = 0.0333},
        new() {InvestmentId = "Investment48878", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-02-22"), Value = 0.013027},
        new() {InvestmentId = "Investment48878", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-04-14"), Value = -0.017632},
        new() {InvestmentId = "Investment48878", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-11-11"), Value = 0.01014},
        new() {InvestmentId = "Investment48878", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-11-24"), Value = 0.008283},

        new() {InvestmentId = "Investment5815", Type = TransactionType.Shares, Date = DateTime.Parse("2016-07-06"), Value = 20.07},
        new() {InvestmentId = "Investment5815", Type = TransactionType.Shares, Date = DateTime.Parse("2016-11-10"), Value = 3.15},
        new() {InvestmentId = "Investment5815", Type = TransactionType.Shares, Date = DateTime.Parse("2017-09-02"), Value = 3.34},
        new() {InvestmentId = "Investment5815", Type = TransactionType.Shares, Date = DateTime.Parse("2018-05-22"), Value = -1.73},
        new() {InvestmentId = "Investment5815", Type = TransactionType.Shares, Date = DateTime.Parse("2018-06-19"), Value = 5.92},


        new() {InvestmentId = "Investment12407", Type = TransactionType.Shares, Date = DateTime.Parse("2016-03-22"), Value = 58},
        new() {InvestmentId = "Investment12407", Type = TransactionType.Shares, Date = DateTime.Parse("2016-07-23"), Value = 7.52},
        new() {InvestmentId = "Investment12407", Type = TransactionType.Shares, Date = DateTime.Parse("2016-08-17"), Value = -5.68},
        new() {InvestmentId = "Investment12407", Type = TransactionType.Shares, Date = DateTime.Parse("2020-01-01"), Value = 12.45},
        new() {InvestmentId = "Investment12407", Type = TransactionType.Shares, Date = DateTime.Parse("2020-01-14"), Value = 26.15},

        new() {InvestmentId = "Investment22216", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-04-04"), Value = 29.07},
        new() {InvestmentId = "Investment22216", Type = TransactionType.Percentage, Date = DateTime.Parse("2016-07-13"), Value = -1.84},
        new() {InvestmentId = "Investment22216", Type = TransactionType.Percentage, Date = DateTime.Parse("2017-02-06"), Value = 5.77},
        new() {InvestmentId = "Investment22216", Type = TransactionType.Percentage, Date = DateTime.Parse("2019-04-23"), Value = 5.08},
        new() {InvestmentId = "Investment22216", Type = TransactionType.Percentage, Date = DateTime.Parse("2019-12-12"), Value = 9.86},

        new() {InvestmentId = "Investment29478", Type = TransactionType.Estate, Date = DateTime.Parse("2018-02-09"), Value = 347060},
        new() {InvestmentId = "Investment29478", Type = TransactionType.Building, Date = DateTime.Parse("2018-02-09"), Value = 1948682},

        new() {InvestmentId = "Investment27611", Type = TransactionType.Estate, Date = DateTime.Parse("2017-05-10"), Value = 340690},
        new() {InvestmentId = "Investment27611", Type = TransactionType.Estate, Date = DateTime.Parse("2017-11-27"), Value = 2020},
        new() {InvestmentId = "Investment27611", Type = TransactionType.Building, Date = DateTime.Parse("2017-05-10"), Value = 152937},
        new() {InvestmentId = "Investment27611", Type = TransactionType.Building, Date = DateTime.Parse("2017-11-27"), Value = 909},

        new() {InvestmentId = "Investment29173", Type = TransactionType.Estate, Date = DateTime.Parse("2017-08-06"), Value = 339853},
        new() {InvestmentId = "Investment29173", Type = TransactionType.Estate, Date = DateTime.Parse("2020-02-15"), Value = -209},
        new() {InvestmentId = "Investment29173", Type = TransactionType.Building, Date = DateTime.Parse("2017-08-06"), Value = 291572},
        new() {InvestmentId = "Investment29173", Type = TransactionType.Building, Date = DateTime.Parse("2020-02-15"), Value = -629},

        new() {InvestmentId = "Investment23835", Type = TransactionType.Shares, Date = DateTime.Parse("2016-07-10"), Value = 12.08},
        new() {InvestmentId = "Investment23835", Type = TransactionType.Shares, Date = DateTime.Parse("2016-07-13"), Value = -6.22},
        new() {InvestmentId = "Investment23835", Type = TransactionType.Shares, Date = DateTime.Parse("2016-11-21"), Value = 4.59},
        new() {InvestmentId = "Investment23835", Type = TransactionType.Shares, Date = DateTime.Parse("2018-09-26"), Value = 4.8},
        new() {InvestmentId = "Investment23835", Type = TransactionType.Shares, Date = DateTime.Parse("2019-01-11"), Value = -1.85},

        new() {InvestmentId = "Investment29481", Type = TransactionType.Estate, Date = DateTime.Parse("2016-12-20"), Value = 255748},
        new() {InvestmentId = "Investment29481", Type = TransactionType.Estate, Date = DateTime.Parse("2020-04-22"), Value = 13},
        new() {InvestmentId = "Investment29481", Type = TransactionType.Building, Date = DateTime.Parse("2016-12-20"), Value = 288439},
        new() {InvestmentId = "Investment29481", Type = TransactionType.Building, Date = DateTime.Parse("2020-04-22"), Value = 1425},

        new() {InvestmentId = "Investment21900", Type = TransactionType.Shares, Date = DateTime.Parse("2016-10-09"), Value = 16.29},
        new() {InvestmentId = "Investment21900", Type = TransactionType.Shares, Date = DateTime.Parse("2017-09-11"), Value = 2.63},
        new() {InvestmentId = "Investment21900", Type = TransactionType.Shares, Date = DateTime.Parse("2017-10-11"), Value = -2.85},
        new() {InvestmentId = "Investment21900", Type = TransactionType.Shares, Date = DateTime.Parse("2019-06-08"), Value = -4.34},
        new() {InvestmentId = "Investment21900", Type = TransactionType.Shares, Date = DateTime.Parse("2019-06-15"), Value = -5.33},

        new() {InvestmentId = "Investment12550", Type = TransactionType.Shares, Date = DateTime.Parse("2016-11-28"), Value = 23.67},
        new() {InvestmentId = "Investment12550", Type = TransactionType.Shares, Date = DateTime.Parse("2018-04-16"), Value = -0.27},
        new() {InvestmentId = "Investment12550", Type = TransactionType.Shares, Date = DateTime.Parse("2018-05-22"), Value = -2.11},
        new() {InvestmentId = "Investment12550", Type = TransactionType.Shares, Date = DateTime.Parse("2018-09-05"), Value = 6.67},
        new() {InvestmentId = "Investment12550", Type = TransactionType.Shares, Date = DateTime.Parse("2019-08-20"), Value = 8.94},


        new() {InvestmentId = "Investment29522", Type = TransactionType.Estate, Date = DateTime.Parse("2017-10-12"), Value = 215559 },
        new() {InvestmentId = "Investment29522", Type = TransactionType.Estate, Date = DateTime.Parse("2018-08-15"), Value = -2043 },
        new() {InvestmentId = "Investment29522", Type = TransactionType.Building, Date = DateTime.Parse("2017-10-12"), Value = 1476211},
        new() {InvestmentId = "Investment29522", Type = TransactionType.Building, Date = DateTime.Parse("2018-08-15"), Value = -14118},


        new() {InvestmentId = "Investment28930", Type = TransactionType.Estate, Date = DateTime.Parse("2016-04-15"), Value = 825550},
        new() {InvestmentId = "Investment28930", Type = TransactionType.Estate, Date = DateTime.Parse("2017-02-05"), Value = 3207},
        new() {InvestmentId = "Investment28930", Type = TransactionType.Estate, Date = DateTime.Parse("2018-10-15"), Value = -3100},
        new() {InvestmentId = "Investment28930", Type = TransactionType.Building, Date = DateTime.Parse("2016-04-15"), Value = 1419683},
        new() {InvestmentId = "Investment28930", Type = TransactionType.Building, Date = DateTime.Parse("2017-02-05"), Value = -5020},
        new() {InvestmentId = "Investment28930", Type = TransactionType.Building, Date = DateTime.Parse("2018-10-15"), Value = -13632}


      };

      return (investments, transactions, quotes);
    }
    #endregion
  }
}