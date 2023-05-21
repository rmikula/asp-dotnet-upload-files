using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoUploadFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploaderController : ControllerBase
    {
        private const long MaxFileSize = 10L * 1024L * 1024L * 1024L; // 10GB, adjust to your need;
        
        private readonly ILogger<FileUploaderController> _logger;

        public FileUploaderController(ILogger<FileUploaderController> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// I can use firstFile or secondFile directly or iterate through Request.Form.Files collection
        /// </summary>
        /// <param name="second">Parameter HAS TO HAVE same name as request. Content-Disposition: form-data; name="secondFile"; filename="input-second.txt"</param>
        [HttpPost]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task PostSingleFile([FromForm] IFormFile firstFile, IFormFile secondFile)
        {

            foreach (var formFile in Request.Form.Files)
            {
                Console.WriteLine($"Field name: {formFile.Name}, FileName: {formFile.FileName}, Size: {formFile.Length} contentType: {formFile.ContentType} ");

                await using var stream = new FileStream($"uploads/{formFile.FileName}", FileMode.Create);
                
                await formFile.CopyToAsync(stream);
            }
        }

        
    }
}
