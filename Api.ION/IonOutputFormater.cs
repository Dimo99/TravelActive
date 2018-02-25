using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Api.ION
{
    public class IonOutputFormater : TextOutputFormatter
    {
        private readonly JsonOutputFormatter jsonOutputFormatter;

        public IonOutputFormater(JsonOutputFormatter jsonOutputFormatter)
        {
            this.jsonOutputFormatter = jsonOutputFormatter;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/ion+json"));
            SupportedEncodings.Add(Encoding.UTF8);
        }
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            return jsonOutputFormatter.WriteResponseBodyAsync(context, selectedEncoding);
        }
    }
}