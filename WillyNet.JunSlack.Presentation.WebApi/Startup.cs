using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WillyNet.JunSlack.Core.Application;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Infraestructure.Persistence;
using WillyNet.JunSlack.Infraestructure.Shared;
using WillyNet.JunSlack.Infraestructure.Shared.Settings;
using WillyNet.JunSlack.Presentation.WebApi.Extensios;
using WillyNet.JunSlack.Presentation.WebApi.Services;
using WillyNet.JunSlack.Presentation.WebApi.SignalR;

namespace WillyNet.JunSlack.Presentation.WebApi
{
    public class Startup
    {
        readonly string myPolicy = "policyChat";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddApplicationLayer();
            services.AddPersistenceInfraestructure(Configuration);
            services.AddSharedInfraestructure(Configuration);
            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
            services.AddApiVersioningExtension();
            services.AddSwaggerExtension();
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddCors(options => options.AddPolicy(myPolicy,
                             builder => builder.WithOrigins(Configuration["Cors:OriginCors"])
                                              .AllowAnyHeader()
                                              .AllowAnyMethod()
                                              .AllowCredentials()
                            ));
            services.AddControllers();            
            services.AddSignalR();
        }
   
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseErrorHandlingMiddleware();
            app.UseRouting();
            app.UseSwaggerExtension();
            app.UseCors(myPolicy);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseEndpoints(endpoints =>
            {               
                endpoints.MapHub<ChatHub>("/chat");
            });
            

        }
    }
}
