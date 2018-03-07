using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TravelActive.Middlewares
{
    public class CookiesMiddleware
    {
        private readonly RequestDelegate next;

        public CookiesMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            RemoveCookie(context);
            await next.Invoke(context);
        }

        private void RemoveCookie(HttpContext context)
        {
            context.Response.Cookies.Delete(".AspNetCore.Identity.Application");
        }
    }
}