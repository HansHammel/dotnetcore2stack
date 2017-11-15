using Microsoft.AspNetCore.Http;

namespace FileUpload2.Models.FileUpload
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
