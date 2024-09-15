using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System.Net.Http.Json;
using System.Net;
using UserManagmentSystem.Controllers;
using UserManagmentSystem.Models;
using UserManagmentSystem.Services;
using Azure;

namespace UserManagementSystem.IntegrationTests
{
    public class AuthControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {   
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;
        private readonly Mock<IUserService> _mockuserService;
        private readonly Mock<IConfiguration> _mockconfiguration;

        private readonly WebApplicationFactory<Program> _factory;

        public AuthControllerTest() {
             _authServiceMock = new Mock<IAuthService>();
            _mockconfiguration = new Mock<IConfiguration>();
            _mockuserService = new Mock<IUserService>();
            _controller = new AuthController(_mockuserService.Object, _mockconfiguration.Object, _authServiceMock.Object);
          }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginModel = new LogIn { Username = "testuser", Password = "password123" };
            _authServiceMock.Setup(service => service.AuthenticateAsync(loginModel.Username, loginModel.Password))
                .ReturnsAsync(new User{ FirstName = "Jaipal", LastName = "Mandala", Username = "Jaipal.Mandala", Password = "Password@123"});

            // Act
            var result = await _controller.Login(loginModel) as OkObjectResult;
            var token = (string)result.Value.GetType().GetProperty("Token").GetValue(result.Value);

            // Assert
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LogIn { Username = "invalidUser", Password = "wrongPassword" };
            _authServiceMock.Setup(service => service.AuthenticateAsync(loginRequest.Username, loginRequest.Password))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(loginRequest) as UnauthorizedObjectResult;



            // Assert
            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            var response = result.Value as Object;
            var message = (string)response.GetType().GetProperty("message").GetValue(response);
            Assert.Equal("Invalid User name or Password", message);    
        }

        [Fact]
        public async Task Login_MissingCredentials_ReturnsBadRequest()
        {
            // Arrange
            LogIn loginRequest = null;

            // Act
            var result = await _controller.Login(loginRequest) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            var response = result.Value as dynamic;
            var message = (string)response.GetType().GetProperty("message").GetValue(response);
            bool success = (bool)response.GetType().GetProperty("success").GetValue(response);
            Assert.Equal("Invalid Credentails", message);
            Assert.False(success);
        }
    }
}