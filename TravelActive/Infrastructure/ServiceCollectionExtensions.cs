using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Api.ION;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TravelActive.Common.Utilities;
using TravelActive.Data;
using TravelActive.Filters;
using TravelActive.Models.Entities;
using TravelActive.Services;

namespace TravelActive.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TravelActiveContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }

        public static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(opt =>
            {
                opt.ClaimsIdentity.UserNameClaimType = Constants.Claims.Username;
                opt.ClaimsIdentity.UserIdClaimType = Constants.Claims.Id;
                opt.ClaimsIdentity.RoleClaimType = "Role";
            });
            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                }).AddEntityFrameworkStores<TravelActiveContext>()
                .AddDefaultTokenProviders();
            return services;
        }

        public static IServiceCollection AddCookieConfigurations(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.FromResult(0);
                    }
                };
            });
            return services;
        }

        public static IServiceCollection AddJwtTokenAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JWTSettings:SecretKey").Value));
            var tokenValidationParameters = new TokenValidationParameters
            {
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                ValidateIssuer = true,
                ValidIssuer = configuration.GetSection("JWTSettings:Issuer").Value,
               
                ValidateAudience = true,
                ValidAudience = configuration.GetSection("JWTSettings:Audience").Value,

                
                ValidateLifetime = true,

                
                ClockSkew = TimeSpan.Zero
            };
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = 
                    new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            Assembly
                .GetAssembly(typeof(Service))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.Name == $"I{t.Name}"))
                .Select(t => new
                {
                    Interface = t.GetInterface($"I{t.Name}"),
                    Implemantation = t
                })
                .ToList()
                .ForEach(s => services.AddTransient(s.Interface, s.Implemantation));
            Assembly
                .GetAssembly(typeof(Service))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().All(i => i.Name != $"I{t.Name}") && t.Name.EndsWith("Service"))
                .ToList()
                .ForEach(c => services.AddTransient(c));
            return services;
        }

        public static IServiceCollection AddMvcConfiguration(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<LinkRewritingFilter>();
                options.Filters.Add<JsonExceptionFilter>();
                var jsonFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().Single();
                options.OutputFormatters.Remove(jsonFormatter);
                options.OutputFormatters.Add(new IonOutputFormater(jsonFormatter));
            });
            return services;
        }
    }
}