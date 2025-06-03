using NUnit.Framework;
using LondonStockApi.Controllers;
using LondonStockApi.Services;
using LondonStockApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LondonStockApi.Tests
{
    [TestFixture]
    public class StocksControllerTests
    {
        [Test]
        public async Task GetStockValue_WithValidTicker_ReturnsValue()
        {
            // Arrange
            var mockService = new Mock<IStockValuationService>();
            mockService.Setup(s => s.GetStockValueAsync("AAPL"))
                .ReturnsAsync(new StockValueViewModel { TickerSymbol = "AAPL", CurrentValue = 150.0m });

            var mockLogger = new Mock<ILogger<StocksController>>();
            var controller = new StocksController(mockService.Object, mockLogger.Object);

            string ticker = "AAPL";

            // Act
            var result = await controller.GetStockValue(ticker);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var value = okResult!.Value as StockValueViewModel;
            Assert.That(value, Is.Not.Null);
            Assert.That(value!.TickerSymbol, Is.EqualTo("AAPL"));
            Assert.That(value.CurrentValue, Is.EqualTo(150.0m));
        }
    }
}
