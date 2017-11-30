using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace FileUpload2.Middleware
{
    public class FileUploadProviderMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IFileProvider fileProvider;

        public FileUploadProviderMiddleware(
            RequestDelegate next,
            IFileProvider fileProvider)
        {
            this.next = next;
            this.fileProvider = fileProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var output = new StringBuilder("");

            IDirectoryContents dir = fileProvider.GetDirectoryContents("");
            foreach (IFileInfo item in dir)
            {
                output.AppendLine(item.Name);
            }

            await context.Response.WriteAsync(output.ToString());
        }
    }
}
