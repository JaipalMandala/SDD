using System.Text.Json;
using UserManagmentSystem.Data;
using UserManagmentSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UserManagmentSystem.Utilities
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        public ExceptionHandler(ILogger<ExceptionHandler> logger,IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception Occured {Message}", exception.Message); // logging the exception

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = exception.Message,
            };

            httpContext.Response.ContentType = MediaTypeNames.Application.ProblemJson;
            httpContext.Response.StatusCode = (int)problemDetails.Status;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            await LogToDatabase(exception);
            return true;
        }

        // Logging exception to database for esay tracking

        private async Task LogToDatabase(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ErrorLogDBContext>();

            var logEntry = new ExceptionsLog
            {
                Timestamp = DateTime.UtcNow,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                ExceptionType = exception.GetType().FullName
            };

            dbContext.exceptionsLogs.Add(logEntry);
            await dbContext.SaveChangesAsync();
        }


    }
}
