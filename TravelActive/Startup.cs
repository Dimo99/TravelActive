using System;
using System.Diagnostics;
using Api.ION;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using TravelActive.Data;
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
            //var context = provider.GetService<TravelActiveContext>();
            //foreach (var contextBusStop in context.BusStops)
            //{
            //    if (contextBusStop.CityId == null || contextBusStop.CityId.Value == 0)
            //    {
            //        double d = double.Parse(contextBusStop.Longitude);
            //        if (d < 27)
            //        {
            //            contextBusStop.CityId = 2;
            //        }
            //        else
            //        {
            //            contextBusStop.CityId = 1;
            //        }
            //    }
            //}

            //context.SaveChanges();
            app.UseMiddleware<CookiesMiddleware>();
            app.UseMiddleware<MakeResponseBodyReadable>();
            app.UseMiddleware<OptionsMiddleware>();
            app.UseMvc();
        }


    }
}
