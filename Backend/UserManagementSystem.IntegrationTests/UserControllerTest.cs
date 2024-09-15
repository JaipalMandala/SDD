using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using UserManagmentSystem.Controllers;
using UserManagmentSystem.Models;
using UserManagmentSystem.Services;
using Xunit;

namespace UserManagementSystem.IntegrationTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        [Fact]
        public async Task CreateUser_UserDoesNotExist_ReturnsOk()
        {
            var addUserRequest = new AddUser { FirstName = "Vamsi", LastName="Krishna", Username = "Vamsi.Krishna", Password = "Password123" , Email="vamsi@test.com",
            CreatedBy = "Jaipal.Mandala", UpdatedBy= "Jaipal.Mandala"};
            var userResponse = new User { Username = "Vamsi.Krishna" };
            _userServiceMock.Setup(service => service.GetUserByNameAsync(addUserRequest.Username))
                .ReturnsAsync((User)null); // User does not exist
            _userServiceMock.Setup(service => service.CreateUserAsync(addUserRequest, addUserRequest.Password))
                .ReturnsAsync(userResponse);

            var result = await _controller.CreateUser(addUserRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as dynamic;
            var message = (string)response.GetType().GetProperty("message").GetValue(response);
            bool success = (bool)response.GetType().GetProperty("success").GetValue(response);
            Assert.Equal("User Created Successfully", message);
            Assert.True(success);
        }

        [Fact]
        public async Task CreateUser_UserAlreadyExists_ReturnsBadRequest()
        {
            var addUserRequest = new AddUser { Username = "Jaipal.Mandala", Password = "Password@123" };
            var existingUser = new User { Username = "Jaipal.Mandala" };
            _userServiceMock.Setup(service => service.GetUserByNameAsync(addUserRequest.Username))
                .ReturnsAsync(existingUser); // User already exists

            var result = await _controller.CreateUser(addUserRequest) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            var response = result.Value as dynamic;
            var message = (string)response.GetType().GetProperty("message").GetValue(response);
            bool success = (bool)response.GetType().GetProperty("success").GetValue(response);
            Assert.Equal("User already exists.", message);
            Assert.False(success);
        }

        [Fact]
        public async Task CreateUser_ServiceThrowsException_ReturnsStatusCode500()
        {
            var addUserRequest = new AddUser { Username = "Jai", Password = "Password@123" };
            _userServiceMock.Setup(x => x.CreateUserAsync(addUserRequest, addUserRequest.Password)).Throws(new System.Exception("Service failed"));

            var result = await _controller.CreateUser(addUserRequest) as ObjectResult;

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

        }

        [Fact]
        public async Task GetUser_UserExists_ReturnsOkWithUser()
        {
            int userId = 4;
            var user = new User { Id = userId, Username = "Jaipal.Mandala" };
            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            
            var result = await _controller.GetUser(userId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as User;
            Assert.Equal(user, response);
        }

        [Fact]
        public async Task GetUser_UserDoesNotExist_ReturnsNotFound()
        {
            int userId = 198;
            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null); // Simulate user not found

            var result = await _controller.GetUser(userId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("User not found", result.Value);
        }

        [Fact]
        public async Task UpdateUser_ValidIdAndUser_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var updateUserRequest = new AddUser { Id = userId, Username = "updatedUser" };
            var updatedUser = new AddUser { Id = userId, Username = "updatedUser" };
            _userServiceMock.Setup(service => service.UpdateUserAsync(updateUserRequest))
                .ReturnsAsync(new User { FirstName = "Jaipal", LastName = "Mandala", Username = "Jaipal.Mandala", Password = "Password@123" });

            var result = await _controller.UpdateUser(userId, updateUserRequest);
          // Assert.IsAssignableFrom<OkObjectResult>(result);

            // Assert that the result value is of the expected type and contains correct data
            var response = result.Result as OkObjectResult;
            var message = (string)response.Value.GetType().GetProperty("message").GetValue(response.Value);
            bool success = (bool)response.Value.GetType().GetProperty("success").GetValue(response.Value);
            Assert.Equal("User Details Updated Successfully", message);
            Assert.True(success);
        }

        [Fact]
        public async Task DeleteUser_ValidId_ReturnsOk()
        {
            // Arrange
            int validUserId = 1;

            // Act
            var result = await _controller.DeleteUser(validUserId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var response = result.Value as dynamic;
            var message = (string)response.GetType().GetProperty("message").GetValue(response);
            bool success = (bool)response.GetType().GetProperty("status").GetValue(response);
            Assert.Equal("User Deleted Successfully",message);
            Assert.True(success);
        }

        [Fact]
        public async Task DeleteUser_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            int invalidUserId = 0; // Using 0 to represent invalid ID

            // Act
            var result = await _controller.DeleteUser(invalidUserId) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            var response = result.Value as dynamic;
            var message = (string)response.GetType().GetProperty("message").GetValue(response);
            bool success = (bool)response.GetType().GetProperty("success").GetValue(response);
            Assert.Equal("User not found", message);
            Assert.False(success);
        }


    }
}

