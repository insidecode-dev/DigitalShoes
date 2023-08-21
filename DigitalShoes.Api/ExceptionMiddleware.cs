﻿using DigitalShoes.Domain.DTOs;
using Newtonsoft.Json;
using System.Net;

namespace DigitalShoes.Api
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ApiResponse
            {
                ErrorMessages = new List<string> { exception.Message },
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
