using WholesalePlatform.WebApi.Middleware;

namespace WholesalePlatform.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static void UseWebApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("/swagger/{documentName}/swagger.json")
                .AllowAnonymous();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Wholesale Platform API v1");
                options.RoutePrefix = "swagger";
            });
        }
    
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health")
            .AllowAnonymous();
    }
}
