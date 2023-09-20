using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CloudStorage.Core.Utils;

namespace CloudStorage.Core.StorageManagers
{
    public class AWSStorageManager
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        private readonly AmazonS3Client _amazonS3Client; 
        private string _bucketName;
        private string _publicHost;

        public string GetStorageName() => "AWS";

        public AWSStorageManager(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            _amazonS3Client = new AmazonS3Client(_configuration.GetSection("AWS:AccessKeyId").Value,
                                                _configuration.GetSection("AWS:SecretAccessKey").Value,
                                                 RegionEndpoint.USEast2);
            LoadConfiguration();
        }
        private void LoadConfiguration() {
            _bucketName = _configuration.GetSection("AWS:BucketName").Value!;
            _publicHost = _configuration.GetSection("AWS:PublicHost").Value!;

        }

        public async Task<string> DownloadAsyncAWS(string photoGuid)
        {
            string fileName = FileExtensionExtractor.ExtractLastSubstring(photoGuid, "/");
            string pathResult = $"{_environment.WebRootPath}/{fileName}";
            var cancelationToken = new CancellationToken();

            if (!File.Exists(pathResult))
            {
                var s3Object = await _amazonS3Client.GetObjectAsync(_bucketName, fileName);
                await s3Object.WriteResponseStreamToFileAsync(pathResult, false, cancelationToken);
            }

            return $"{EnvironmentManager.GetApplicationHost()}/{fileName}";
        }

        public async Task<string> UploadAsyncAWS(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + FileExtensionExtractor.Extract(file.FileName);
            var putObjectRequest = await GetPutObjectRequestAsync(file, fileName);

            await _amazonS3Client.PutObjectAsync(putObjectRequest);

            return $"https://{_bucketName}.{_publicHost}/{fileName}";
        }

        private async Task<PutObjectRequest> GetPutObjectRequestAsync(IFormFile file, string fileName)
        {
            var stream = new MemoryStream();

            await file.CopyToAsync(stream);

            return new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream
            };
        }
    }
}

