using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

namespace DemoUploadFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploaderController : ControllerBase
    {
        private const long MaxFileSize = 10L * 1024L * 1024L * 1024L; // 10GB, adjust to your need;

        private readonly ILogger<FileUploaderController> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IAzureClientFactory<BlobServiceClient> _azureClientFactory;

        public FileUploaderController(ILogger<FileUploaderController> logger, BlobServiceClient blobServiceClient, IAzureClientFactory<BlobServiceClient> azureClientFactory)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
            _azureClientFactory = azureClientFactory;
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
                Console.WriteLine(
                    $"Field name: {formFile.Name}, FileName: {formFile.FileName}, Size: {formFile.Length} contentType: {formFile.ContentType} ");

                await using var stream = new FileStream($"uploads/{formFile.FileName}", FileMode.Create);

                await formFile.CopyToAsync(stream);
            }
        }

        [HttpPost("toBlob")]
        public async Task PostFileToAzureBlob([FromForm] IFormFile firstFile)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("files");

            // Check if container exists
            var response = await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(firstFile.FileName);

            // set http headers
            var headers = new BlobHttpHeaders()
            {
                ContentType = firstFile.ContentType,
            };

            var uploadAsync = await blobClient.UploadAsync(content: firstFile.OpenReadStream(), headers);

            // Response<BlobContentInfo>? uploadAsync = await blobClient.UploadAsync(content: firstFile.OpenReadStream(), overwrite: true);
            // await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = firstFile.ContentType });

            // await containerClient.UploadBlobAsync(firstFile.FileName, firstFile.OpenReadStream());

            Accepted();
        }

        [HttpPost("toBlob2")]
        public async Task PostFileToAzureBlob2([FromForm] IFormFile firstFiles)
        {
            var client = _azureClientFactory.CreateClient("RoMikVersion");

            var containerClient = client.GetBlobContainerClient("files");
            
            foreach (var blobItem in containerClient.GetBlobs())
            {
                Console.WriteLine($"BlobName: {blobItem.Name} contentType: {blobItem.Properties.ContentType}");
            }
        }
    }
}