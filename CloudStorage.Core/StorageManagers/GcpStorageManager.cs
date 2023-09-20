using CloudStorage.Core.Utils;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using System;


namespace CloudStorage.Core
{
    public class GcpStorageManager
    {
        private readonly StorageClient _client;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        private string _bucketName = "";
        private string _publicHost = "";

        public string GetStorageName() => "GCP";

        public GcpStorageManager(IWebHostEnvironment environment, IConfiguration configuration)
        {
#if (DEBUG)
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $"{environment.ContentRootPath}\\sa-key\\{configuration.GetSection("GCS:ServiceAccountKeyFilename").Value}");
#endif
            _client = StorageClient.Create();
            _environment = environment;
            _configuration = configuration;

            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            _bucketName = _configuration.GetSection("GCS:BucketName").Value!.ToString();
            _publicHost = _configuration.GetSection("GCS:PublicHost").Value!.ToString();
        }

        public async Task<string> UploadAsyncGcp(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + FileExtensionExtractor.Extract(file.FileName);

            var stream = new MemoryStream();

            await file.CopyToAsync(stream);

            await _client.UploadObjectAsync(_bucketName, fileName, file.ContentType, stream);

            return $"{_publicHost}/{_bucketName}/{fileName}";
        }

        public async Task<string> DownloadAsyncGcp(string photoGuid)
        {
            string fileName = FileExtensionExtractor.ExtractLastSubstring(photoGuid, "/");
            string pathResult = $"{_environment.WebRootPath}/{fileName}";

            if (!File.Exists(pathResult))
            {
                using FileStream fs = new(pathResult, FileMode.Create);
                await _client.DownloadObjectAsync(_bucketName, fileName, fs);
            }

            return $"{EnvironmentManager.GetApplicationHost()}/{fileName}";
        }
    }
}
