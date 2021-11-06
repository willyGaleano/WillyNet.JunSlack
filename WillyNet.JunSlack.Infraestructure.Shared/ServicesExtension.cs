using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WillyNet.JunSlack.Core.Application.Interface;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Infraestructure.Shared.Services;

namespace WillyNet.JunSlack.Infraestructure.Shared
{
    public static class ServicesExtension
    {
        public static void AddSharedInfraestructure(this IServiceCollection services, IConfiguration _config)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();                                    
            services.AddScoped<IMediaUpload, MediaUpload>();           
        }
    }
}
