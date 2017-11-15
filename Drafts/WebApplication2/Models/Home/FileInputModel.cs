using Microsoft.AspNetCore.Http;

namespace WebApplication2.Models.Home
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
