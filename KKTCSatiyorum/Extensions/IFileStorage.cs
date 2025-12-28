namespace KKTCSatiyorum.Extensions
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct = default);
        void Delete(string relativePath);
        bool IsValidImage(IFormFile file);
    }
}
