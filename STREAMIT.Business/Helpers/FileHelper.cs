using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Helpers
{
    public static class FileHelper
    {
        public static bool CheckSize(this IFormFile file, int mb)
        {
            return file.Length <= mb * 1024 * 1024;
        }

        public static bool CheckType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }
    }
}
