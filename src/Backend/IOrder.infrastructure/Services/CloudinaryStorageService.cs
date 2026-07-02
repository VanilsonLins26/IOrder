using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using IOrder.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace IOrder.infrastructure.Services;

internal class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IConfiguration configuration)
    {
        var cloudName = configuration.GetValue<string>("Cloudinary:CloudName");
        var apiKey = configuration.GetValue<string>("Cloudinary:ApiKey");
        var apiSecret = configuration.GetValue<string>("Cloudinary:ApiSecret");
        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, fileStream)
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult.Error != null)
        {
            throw new System.Exception(uploadResult.Error.Message);
        }
        return uploadResult.SecureUrl.ToString();
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return;

        var publicId = ExtractPublicId(imageUrl);
        if (string.IsNullOrEmpty(publicId)) return;

        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (result.Error != null)
        {
            throw new Exception($"Erro ao deletar imagem no Cloudinary: {result.Error.Message}");
        }
    }

    private string ExtractPublicId(string imageUrl)
    {
        int uploadIndex = imageUrl.IndexOf("upload/");
        if (uploadIndex == -1) return null;

   
        string afterUpload = imageUrl.Substring(uploadIndex + 7);

        if (afterUpload.StartsWith("v"))
        {
            int slashIndex = afterUpload.IndexOf("/");
            if (slashIndex != -1)
            {
                afterUpload = afterUpload.Substring(slashIndex + 1);
            }
        }

        int dotIndex = afterUpload.LastIndexOf(".");
        if (dotIndex != -1)
        {
            afterUpload = afterUpload.Substring(0, dotIndex);
        }

        return afterUpload;
    }


}
