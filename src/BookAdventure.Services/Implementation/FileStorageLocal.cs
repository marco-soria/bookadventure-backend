using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookAdventure.Services.Implementation;

public class FileStorageLocal : IFileStorage
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<FileStorageLocal> _logger;

    public FileStorageLocal(
        IWebHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor,
        ILogger<FileStorageLocal> logger)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        try
        {
            // Create unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            
            // Create folder path
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
            
            // Ensure directory exists
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Create full file path
            var filePath = Path.Combine(folderPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path
            var relativePath = Path.Combine("uploads", folder, fileName).Replace("\\", "/");
            _logger.LogInformation("File saved successfully at: {FilePath}", relativePath);
            
            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {FileName}", file.FileName);
            throw;
        }
    }

    public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
    {
        try
        {
            // Create unique filename
            var fileName = $"{Guid.NewGuid()}.{extension}";
            
            // Create folder path
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads", container);
            
            // Ensure directory exists
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Create full file path
            var filePath = Path.Combine(folderPath, fileName);

            // Save file
            await File.WriteAllBytesAsync(filePath, content);

            // Return relative path
            var relativePath = Path.Combine("uploads", container, fileName).Replace("\\", "/");
            _logger.LogInformation("File saved successfully at: {FilePath}", relativePath);
            
            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file with extension: {Extension}", extension);
            throw;
        }
    }

    public async Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType)
    {
        try
        {
            // Delete old file if exists
            await DeleteFile(path, container);

            // Save new file
            return await SaveFile(content, extension, container, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing file: {Path}", path);
            throw;
        }
    }

    public async Task DeleteFile(string path, string container)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                return;

            var filePath = Path.Combine(_environment.WebRootPath, path);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FilePath}", path);
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}", path);
            throw;
        }
    }

    public bool FileExists(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        var fullPath = Path.Combine(_environment.WebRootPath, filePath);
        return File.Exists(fullPath);
    }

    public string GetFileUrl(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return string.Empty;

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return filePath;

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/{filePath}";
    }
}
