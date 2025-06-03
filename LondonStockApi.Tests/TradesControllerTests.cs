using NUnit.Framework;
using LondonStockApi.Controllers;
using LondonStockApi.Services;
using LondonStockApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace LondonStockApi.Tests
{
    [TestFixture]
    public class TradesControllerTests
    {
        [Test]
        public async Task SubmitTrade_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var mockService = new Mock<ITradeService>();
            mockService.Setup(s => s.RecordTradeAsync(It.IsAny<TradeInputModel>(), "broker1"))
                .ReturnsAsync(Guid.NewGuid());

            var mockLogger = new Mock<ILogger<TradesController>>();
            var controller = new TradesController(mockService.Object, mockLogger.Object);

            // Simulate authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("broker_id", "broker1")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var tradeInput = new TradeInputModel { TickerSymbol = "AAPL", Price = 100, Shares = 10, BrokerId = "broker1" };

            // Act
            var result = await controller.RecordTrade(tradeInput);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        }
    }
}
