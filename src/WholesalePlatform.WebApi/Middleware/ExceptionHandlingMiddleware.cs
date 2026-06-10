using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WholesalePlatform.Application.Common.Exceptions;
using WholesalePlatform.Domain.Common;

namespace WholesalePlatform.WebApi.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private const string PostgreSqlUniqueViolation = "23505";
    private const string PostgreSqlCheckViolation = "23514";

    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteProblemAsync(context, exception);
        }
    }

    private static Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var title = GetTitle(exception);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = context.Request.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            problem.Extensions["errors"] = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        return context.Response.WriteAsJsonAsync(problem);
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            DbUpdateException { InnerException: PostgresException { SqlState: PostgreSqlUniqueViolation } } =>
                StatusCodes.Status409Conflict,
            DbUpdateException { InnerException: PostgresException { SqlState: PostgreSqlCheckViolation } } =>
                StatusCodes.Status400BadRequest,
            DomainException => StatusCodes.Status409Conflict,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetTitle(Exception exception)
    {
        return exception switch
        {
            DbUpdateConcurrencyException =>
                "Resource was modified by another request. Reload it and try again.",
            DbUpdateException { InnerException: PostgresException { SqlState: PostgreSqlUniqueViolation } } =>
                "Resource with the same unique value already exists.",
            DbUpdateException { InnerException: PostgresException { SqlState: PostgreSqlCheckViolation } } =>
                "Request violates database constraints.",
            UnauthorizedAccessException => "Unauthorized",
            ForbiddenAccessException => "Forbidden",
            KeyNotFoundException => "Resource not found.",
            ValidationException => "Validation failed.",
            DomainException => exception.Message,
            InvalidOperationException => exception.Message,
            _ => "Unexpected server error."
        };
    }
}
