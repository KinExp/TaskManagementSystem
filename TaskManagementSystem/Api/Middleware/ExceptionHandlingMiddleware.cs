using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using TaskManagement.Api.Models;
using TaskManagement.Application.Exceptions;

namespace TaskManagement.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                await HandleAppException(context, ex);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var message = string.Join("; ",
                    ex.Errors.Select(e => e.ErrorMessage));

                await WriteRespone(
                    context,
                    HttpStatusCode.BadRequest,
                    "validation_error",
                    message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await WriteRespone(
                    context,
                    HttpStatusCode.InternalServerError,
                    "internal_server_error",
                    "An unexpected error occurred");
            }
        }

        private async Task HandleAppException(
            HttpContext context,
            AppException exception)
        {
            HttpStatusCode statusCode = exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                ValidationException => HttpStatusCode.BadRequest,
                ForbiddenException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.BadRequest
            };

            string type = exception switch
            {
                NotFoundException => "not_found",
                ValidationException => "validation_error",
                ForbiddenException => "forbidden",
                _ => "app_error"
            };

            await WriteRespone(context, statusCode, type, exception.Message);
        }

        private static async Task WriteRespone(
            HttpContext context,
            HttpStatusCode statusCode,
            string type,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Type = type,
                Message = message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
