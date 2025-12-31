using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace KKTCSatiyorum.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, 
            ILogger<GlobalExceptionMiddleware> logger, 
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu: {Message}", ex.Message);

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response zaten başladığı için exception handling atlanıyor.");
                    throw;
                }

                if (IsAjaxRequest(context))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var problemDetails = new ProblemDetails
                    {
                        Status = context.Response.StatusCode,
                        Title = "Sunucu Hatası",
                        Detail = _env.IsDevelopment() ? ex.ToString() : "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                        Instance = context.Request.Path
                    };

                    problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                    var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
                }
                else
                {
                    throw;
                }
            }
        }

        private bool IsAjaxRequest(HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   context.Request.Headers["Accept"].ToString().Contains("application/json");
        }
    }
}
