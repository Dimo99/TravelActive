using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace TravelActive.Middlewares
{
    public class SuccessMiddleware
    {
        private readonly RequestDelegate next;

        public SuccessMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            AddSuccessProperty(context);
            await next.Invoke(context);
        }

        private void AddSuccessProperty(HttpContext context)
        {
            string initialResponse;
            using (StreamReader streamReader = new StreamReader(context.Response.Body))
            {
                initialResponse = streamReader.ReadToEnd();
            }

            JObject jObject = initialResponse != "" ? JObject.Parse(initialResponse) : new JObject();
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                jObject.Add("success",true);
            }
            else
            {
                jObject.Add("success",false);
            }

            string final = jObject.ToString(Newtonsoft.Json.Formatting.None);
            byte[] b = Encoding.UTF8.GetBytes(final);
            using (MemoryStream memoryStream = new MemoryStream(b))
            {
                context.Response.Body = memoryStream;
            }

        }
    }
}