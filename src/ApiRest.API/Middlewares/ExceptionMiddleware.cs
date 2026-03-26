using System.Net;
using System.Text.Json;
using ApiRest.Domain.Exceptions;
using FluentValidation;

namespace ApiRest.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            await WriteAsync(ctx, (int)HttpStatusCode.UnprocessableEntity,
                "Validation failed", errors);
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(ctx, (int)HttpStatusCode.NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            await WriteAsync(ctx, (int)HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
            var response = new
            {
                status = 500,
                message = ex.Message, 
                stack = ex.StackTrace 
            };

            ctx.Response.StatusCode = 500;
            await ctx.Response.WriteAsJsonAsync(response);
        }
    }

    private static async Task WriteAsync(
        HttpContext ctx, int status, string message,
        object? errors = null)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new
        {
            status,
            message,
            errors,
            timestamp = DateTime.UtcNow
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await ctx.Response.WriteAsync(body);
    }
}