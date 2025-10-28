using Microsoft.AspNetCore.Http;

namespace LinkUp.Application.Interfaces.Storage
{
    public interface IFileStorage
    {
        Task<string> SavePostImageAsync(IFormFile file, long maxBytes);
    }
}
