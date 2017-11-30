using Microsoft.AspNetCore.Http;

namespace RefArc.Mvc.FileUpload.Models.Home
{
    public class FileInputModel
    {
        public IFormFile FileToUpload { get; set; }
    }
}
