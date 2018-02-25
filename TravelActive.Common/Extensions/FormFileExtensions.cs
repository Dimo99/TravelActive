using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TravelActive.Common.Extensions
{
    public static class FormFileExtensions
    {
        public static string GetFileType(this IFormFile formFile)
        {
            int lastDotIndex = formFile.FileName.LastIndexOf('.');
            return formFile.FileName.Substring(lastDotIndex + 1);
        }

        public static async Task<byte[]> ToByteArrayAsync(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}