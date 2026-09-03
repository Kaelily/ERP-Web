using System.Net;
using System.Text.Json;
using ERP.Application.Common;
using FluentValidation;

namespace ERP.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação de modelo");
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

            var problem = new ProblemDetailsDto
            {
                Title = "Erro de validação",
                Status = 422,
                Detail = "Um ou mais campos contêm valores inválidos.",
                Errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno não tratado");
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new ProblemDetailsDto
            {
                Title = "Erro interno do servidor",
                Status = 500,
                Detail = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
