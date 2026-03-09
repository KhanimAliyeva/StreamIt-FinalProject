using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using STREAMIT.Business.Dtos.CloudinaryDtos;
using STREAMIT.Business.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace STREAMIT.Business.Services.Implementations
{
    internal class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;
        private readonly CloudinaryOptionsDto _optionsDto;
        private readonly Cloudinary _cloudinary = null!;

        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;
            _optionsDto = _configuration.GetSection("CloudinarySettings").Get<CloudinaryOptionsDto>() ?? new();

            var myAccount = new Account { ApiKey = _optionsDto.ApiKey, ApiSecret = _optionsDto.ApiSecret, Cloud = _optionsDto.CloudName };

            _cloudinary = new Cloudinary(myAccount);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> FileCreateAsync(IFormFile file)
        {
            string fileName = string.Concat(Guid.NewGuid(), file.FileName.Substring(file.FileName.LastIndexOf('.')));

            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = "StreamIt"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            string url = uploadResult.SecureUrl.ToString();

            return url;
        }

        public async Task<bool> FileDeleteAsync(string filePath)
        {
            try
            {
                string publicIdWithExtension = filePath.Substring(filePath.LastIndexOf("StreamIt"));
                string publicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));

                var deleteParams = new DelResParams()
                {
                    PublicIds = new List<string> { publicId },
                    Type = "upload",
                    ResourceType = ResourceType.Image
                };
                var result = await _cloudinary.DeleteResourcesAsync(deleteParams);

                return result.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
