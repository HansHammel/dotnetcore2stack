using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using FileUpload2.Models.FileUpload;
using Microsoft.Extensions.FileProviders;

namespace FileUpload2.Controllers
{
    //[Route("api/v1/[controller]/[action]")]
    [Route("api/v1/[action]")]
    public class FileUploadController : Controller
    {
        private readonly IFileProvider fileProvider;

        public FileUploadController(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        // test: curl -X POST -H "Content-Type: multipart/form-data" -F "name=file" -F "file=@somefile.txt" http://localhost:5001/api/v1/fileupload
        // note: the parameters and the form field names must match: file=@somefile.txt -> IFormFile file
        //[Route("uploadfile")]
        [ActionName("fileupload")]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // TODO: try to remove this in final version
            try
            {
                if (file == null || file.Length == 0)
                    return Content("file not selected");

                //TODO: catch System.IO.DirectoryNotFoundException -> 404, etc.
                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot",
                            Path.GetFileName(file.FileName));

                //TODO: use a temp folder then move file
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return RedirectToAction("Files");
            }
            // better to use finally here but this would not complete the request, exception to the rule!
            catch (Exception e)
            {
                // TODO: maybe delete file!
                // log exceptions
                // LogException(e);
                // complete the request so we see errors in th gui/client
                return StatusCode(500, e.ToString());
                // rethrow exception
                throw; 
            }
        }

        // test: curl.exe" -X POST -H "Content-Type: multipart/form-data" -F "files=@first.txt" -F "files=@second.txt" http://localhost:5001/api/v1/fileuploads
        [ActionName("fileuploads")]
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return Content("files not selected");

            foreach (var file in files)
            {
                var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        Path.GetFileName(file.FileName));

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return RedirectToAction("Files");
        }

        [ActionName("uploadcustom")]
        [HttpPost]
        public async Task<IActionResult> UploadFileViaModel(FileInputModel model)
        {
            if (model == null ||
                model.FileToUpload == null || model.FileToUpload.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        Path.GetFileName(model.FileToUpload.FileName));

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.FileToUpload.CopyToAsync(stream);
            }

            return RedirectToAction("Files");
        }
        
        
        
        public IActionResult Files()
        {
            var model = new FilesViewModel();
            foreach (var item in this.fileProvider.GetDirectoryContents(""))
            {
                model.Files.Add(
                    new FileDetails { Name = item.Name, Path = item.PhysicalPath });
            }
            //return View(model);
            return Ok(model);
        }
        

        [HttpGet]
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
            {
                var model = new FilesViewModel();
                foreach (var item in this.fileProvider.GetDirectoryContents(""))
                {
                    model.Files.Add(
                        new FileDetails { Name = item.Name, Path = item.PhysicalPath });
                }
                //return View(model);
                return Ok(model);
            }
            //return Content("filename not present");
            else
            {
                var path = Path.Combine(
                               Directory.GetCurrentDirectory(),
                               "wwwroot", filename);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            //return StatusCode(500);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
