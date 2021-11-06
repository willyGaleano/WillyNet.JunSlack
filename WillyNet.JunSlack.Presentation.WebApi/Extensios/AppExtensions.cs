using Microsoft.AspNetCore.Builder;
using WillyNet.JunSlack.Presentation.WebApi.Middlewares;

namespace WillyNet.JunSlack.Presentation.WebApi.Extensios
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WillyNet.JunSlackApi");
            });
        }
        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
