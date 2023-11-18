using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleWare> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleWare(RequestDelegate Next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment environment)
        {
            _next = Next;
            _logger = logger;
            _environment = environment;
        }

        // InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // Production => Log ex in Database
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //if (_environment.IsDevelopment())
                //{
                //    var Response = new ApiExceptionResponse(500, ex.Message, ex.StackTrace.ToString());
                //}
                //else
                //{
                //    //var Response = new ApiExceptionResponse(500);
                //    //var Response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
                //}
                var Response = _environment.IsDevelopment() ? new ApiExceptionResponse(500, ex.Message, ex.StackTrace.ToString()) : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
                var Options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var JsonResponse = JsonSerializer.Serialize(Response);
                context.Response.WriteAsync(JsonResponse);
            }
        }
    }
}
