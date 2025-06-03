using NUnit.Framework;
using NUnit.Framework.Constraints;
using LondonStockApi.Controllers;
using LondonStockApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;

namespace LondonStockApi.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        [Test]
        public void Token_WithValidCredentials_ReturnsToken()
        {
     
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "THIS_IS_A_DEMO_SECRET_KEY_FOR_TESTING_PURPOSES_ONLY_123456"},
                {"Jwt:Issuer", "https://test-issuer.com"},
                {"Jwt:Audience", "https://test-audience.com/api"},
                {"Jwt:ExpiryInMinutes", "60"},
                {"DemoUsers:0:Username", "broker1"},
                {"DemoUsers:0:Password", "Password123!"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var controller = new AuthController(configuration);

            var loginModel = new LoginModel { Username = "broker1", Password = "Password123!" };

            // Act
            var result = controller.GenerateToken(loginModel);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.Value, Is.Not.Null);
            NUnit.Framework.StringAssert.Contains("token", okResult.Value.ToString()!.ToLower());
        }
    }
}
