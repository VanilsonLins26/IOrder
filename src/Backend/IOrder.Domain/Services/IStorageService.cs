using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Services;

public interface IStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName);

    Task DeleteImageAsync(string imageUrl);
}