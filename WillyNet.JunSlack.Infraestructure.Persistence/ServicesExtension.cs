using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using WillyNet.JunSlack.Core.Application.Interface.Repositories;
using WillyNet.JunSlack.Core.Application.Interfaces;
using WillyNet.JunSlack.Core.Domain.Entities;
using WillyNet.JunSlack.Core.Domain.Settings;
using WillyNet.JunSlack.Infraestructure.Persistence.Contexts;
using WillyNet.JunSlack.Infraestructure.Persistence.Repositories;
using WillyNet.JunSlack.Infraestructure.Persistence.Services;

namespace WillyNet.JunSlack.Infraestructure.Persistence
{
    public static class ServicesExtension
    {
        public static void AddPersistenceInfraestructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region CONTEXTS
            services.AddDbContext<ApplicationDbContext>(
                   options => options.UseSqlServer(
                         configuration.GetConnectionString("DefaultConnection"),
                         b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                  )
                );
            #endregion

            #region IDENTITY
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };

                    /*
                    En las API web estándar, los tokens de portador se envían en un encabezado HTTP. 
                    Sin embargo, SignalR no puede configurar estos encabezados en los navegadores cuando 
                    se utilizan algunos transportes.
                    Cuando se utilizan WebSockets y Eventos enviados por el servidor, el token se transmite 
                    como un parámetro de cadena de consulta.
                     */

                    o.Events = new JwtBearerEvents
                    {

                        OnMessageReceived = context =>
                        {
                            //para pasar el token por query string
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                                context.Token = accessToken;

                            return Task.CompletedTask;
                        },

                        
                    };

                });
            #endregion

            #region REPOSITORIES
            services.AddTransient(typeof(IRepositoryGenericSpecification<>), typeof(RepositoryGenericSpecification<>));
            #endregion

            #region SERVICES
            services.AddTransient<IAccountService, AccountService>();            
            #endregion
        }
    }
}
