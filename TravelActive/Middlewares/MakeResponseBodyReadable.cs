﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace TravelActive.Middlewares
{
    public class MakeResponseBodyReadable
    {
        private readonly RequestDelegate next;

        public MakeResponseBodyReadable(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var existingBody = context.Response.Body;
            var buffer = new MemoryStream();
            context.Response.Body = buffer;
            await next(context);
            if (context.Request.Method == "OPTIONS")
            {
                return;
            }
            context.Response.Headers.Add("Access-Control-Allow-Origin","*");
            buffer.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(buffer);
            if (context.Response.ContentType != "application/json")
            {
                context.Response.ContentType = "application/json";
            }
            string responseBody = await reader.ReadToEndAsync();
            buffer.Dispose();
            context.Response.Body = existingBody;
            JObject jObject = responseBody == "" ? new JObject() : JObject.Parse(responseBody);
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                jObject.Add("success",true);
            }
            else
            {
                jObject.Add("success",false);
            }
            await context.Response.WriteAsync(jObject.ToString(Newtonsoft.Json.Formatting.None));
        }
        
    }
}