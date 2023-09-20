
namespace CloudStorage.Core.Utils
{
    public class FileExtensionExtractor
    {
        public static string Extract(string fileName)
        {
            if (!fileName.Contains('.'))
                return "";

            var lastIndexOfPeriod = fileName.LastIndexOf(".");

            return fileName.Substring(lastIndexOfPeriod);
        }

        public static string ExtractLastSubstring(string fileName, string separator)
        {
            int lastSlashIndex = fileName.LastIndexOf(separator);
            return fileName.Substring(lastSlashIndex + 1);
        }

    }

}
