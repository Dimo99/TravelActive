using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace TravelActive.Models.ViewModels
{
    public class ApiError
    {
        public ApiError()
        {

        }
        
        public ApiError(string error)
        {
            Message = "Opps";
            Detail = error;
        }
        public ApiError(ModelStateDictionary modelState)
        {
            Message = "Invalid parrameters";
            Detail = modelState
                .FirstOrDefault(x => x.Value.Errors.Any())
                .Value.Errors.FirstOrDefault()
                ?.ErrorMessage;
        }
        public string Message { get; set; }
        public string Detail { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("")]
        public string StackTrace { get; set; }
    }
}