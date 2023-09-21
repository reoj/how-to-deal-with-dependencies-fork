using Microsoft.AspNetCore.Http;

namespace CloudStorage.Core
{
    public interface IStorageManager
    {
        Task<string> DownloadAsync(string photoGuid);
        string GetStorageName();
        Task<string> UploadAsync(IFormFile file);
    }
}