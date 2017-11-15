using Microsoft.AspNetCore.Http;

namespace Upload.Models.Home
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
