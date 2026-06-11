using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymMangmentSystem.BLL.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly string[] _allowedExtenstions = { ".jpg", ".jpeg", ".png" };
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(IWebHostEnvironment env, ILogger<AttachmentService> logger)
        {
            _logger = logger;
            _env = env;
        }

        public async Task<string?> UploadAsync(Stream FileStream, string FileName, string FolderName, CancellationToken ct = default)
        {
            var extension = Path.GetExtension(FileName).ToLowerInvariant();
            if (!_allowedExtenstions.Contains(extension))
            {
                _logger.LogWarning("Rejected upload of {FileName}: extension {Extension} is not allowed.", FileName, extension);
                return null;
            }

            if (FileStream.Length > _maxFileSize)
            {
                _logger.LogWarning("Rejected upload of {FileName}: size {Size} exceeds the {Limit} byte limit.", FileName, FileStream.Length, _maxFileSize);
                return null;
            }

            var folderPath = Path.Combine(_env.WebRootPath, FolderName);
            Directory.CreateDirectory(folderPath);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            try
            {
                using var target = new FileStream(fullPath, FileMode.Create);
                await FileStream.CopyToAsync(target, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save uploaded file {FileName}.", FileName);
                return null;
            }

            // Web-relative path (served from wwwroot) — e.g. /images/MembersPictures/<guid>.jpg
            return $"/{FolderName.Replace('\\', '/')}/{uniqueFileName}";
        }

        public bool Delete(string FileName, string FolderName)
        {
            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, FolderName, FileName);
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("Delete skipped: file {FullPath} does not exist.", fullPath);
                    return false;
                }

                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file {FileName} from {FolderName}.", FileName, FolderName);
                return false;
            }
        }

        public (Stream stream, string ContentType)? GetFile(string FileName, string FolderName)
        {
            var fullPath = Path.Combine(_env.WebRootPath, FolderName, FileName);
            if (!File.Exists(fullPath))
                return null;

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return (stream, GetContentType(FileName));
        }

        private static string GetContentType(string fileName) => Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}
