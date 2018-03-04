using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TravelActive.Middlewares
{
    public class CrossOriginMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Invoke(context);
            await next.Invoke(context);
        }

        private void Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Headers","*");
        }
    }
}