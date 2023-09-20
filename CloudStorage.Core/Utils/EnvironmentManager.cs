using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorage.Core.Utils
{
    public class EnvironmentManager
    {
        public static string GetApplicationHost()
        {
            var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";");

            if (url.Length == 0)
                return "http://localhost";

            return url[0];
        }
    }
}