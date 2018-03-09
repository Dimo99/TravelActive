using System;
using Api.ION;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using TravelActive.Infrastructure;
using TravelActive.Middlewares;
using TravelActive.Services;

namespace TravelActive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseConfigurations(Configuration);
            services.AddIdentityConfigurations();
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddJwtTokenAuthorization(Configuration);
            services.AddCookieConfigurations();
            services.AddDomainServices();
            services.AddAutoMapper();
            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("SMTP"));
            services.Configure<ApiOptions>(Configuration.GetSection("Api"));
            services.Configure<PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));
            services.AddMvcConfiguration();
            services.AddSingleton<OptionsMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            app.UseMiddleware<MakeResponseBodyReadable>();
            app.UseMiddleware<OptionsMiddleware>();
            app.UseMvc();
        }

        
    }
}
