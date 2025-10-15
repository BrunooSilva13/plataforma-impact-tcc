using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Middleware
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
                _logger.LogError(ex, "Erro não tratado");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = "Recurso não encontrado";
                    break;

                case NpgsqlException pgEx when pgEx.SqlState == "23505": // violação de unique constraint
                    status = HttpStatusCode.Conflict;
                    message = "Já existe um registro com esses dados";
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Ocorreu um erro interno no servidor";
                    break;
            }

            var response = new
            {
                status = (int)status,
                error = status.ToString(),
                message,
                traceId = context.TraceIdentifier // bom para correlacionar logs
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
