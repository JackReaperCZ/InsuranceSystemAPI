namespace InsuranceSystemAPI.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        bool IsValidFileType(string fileName, string[] allowedExtensions);
        bool IsValidFileSize(long fileSize, long maxSizeInBytes);
        string GetContentType(string fileName);
    }
}