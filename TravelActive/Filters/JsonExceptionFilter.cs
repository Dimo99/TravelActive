using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TravelActive.Models.ViewModels;

namespace TravelActive.Filters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        public JsonExceptionFilter(IHostingEnvironment env)
        {
            this.env = env;
        }
        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();
            if (env.IsDevelopment())
            {
                error.Message = context.Exception.Message;
                error.Detail = context.Exception.StackTrace;
            }
            else
            {
                error.Message = "A server error occured";
                error.Detail = context.Exception.Message;
            }
            context.Result = new ObjectResult(error)
            {
                StatusCode = 500
            };
        }
    }
}