using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TravelActive.Middlewares
{
    public class OptionsMiddleware : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!await BeginInvoke(context))
            {
                await next.Invoke(context);
            }
        }

        private async Task<bool> BeginInvoke(HttpContext context)
        {
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept, Authorization" });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
                context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("OK");
                return true;
            }

            return false;
        }
    }
}