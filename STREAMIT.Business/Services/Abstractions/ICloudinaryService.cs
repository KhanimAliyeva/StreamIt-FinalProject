using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface ICloudinaryService
    {
        Task<string> FileCreateAsync(IFormFile file, string resourceType = "image");
        Task<bool> FileDeleteAsync(string filePath);
        
    }
}
