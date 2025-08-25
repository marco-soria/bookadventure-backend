using Microsoft.AspNetCore.Http;

namespace BookAdventure.Services.Interfaces;

public interface IFileStorage
{
    /// <summary>
    /// Saves a file to the storage and returns the file path
    /// </summary>
    /// <param name="file">The file to save</param>
    /// <param name="folder">The folder where to save the file</param>
    /// <returns>The relative path to the saved file</returns>
    Task<string> SaveFileAsync(IFormFile file, string folder);

    /// <summary>
    /// Saves file content to storage
    /// </summary>
    /// <param name="content">File content as byte array</param>
    /// <param name="extension">File extension</param>
    /// <param name="container">Container/folder name</param>
    /// <param name="contentType">MIME content type</param>
    /// <returns>The relative path to the saved file</returns>
    Task<string> SaveFile(byte[] content, string extension, string container, string contentType);

    /// <summary>
    /// Updates an existing file
    /// </summary>
    /// <param name="content">New file content</param>
    /// <param name="extension">File extension</param>
    /// <param name="container">Container/folder name</param>
    /// <param name="path">Existing file path</param>
    /// <param name="contentType">MIME content type</param>
    /// <returns>The path to the updated file</returns>
    Task<string> EditFile(byte[] content, string extension, string container, string path, string contentType);

    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="path">The path of the file to delete</param>
    /// <param name="container">Container/folder name</param>
    Task DeleteFile(string path, string container);

    /// <summary>
    /// Checks if a file exists in storage
    /// </summary>
    /// <param name="filePath">The path of the file to check</param>
    /// <returns>True if file exists</returns>
    bool FileExists(string filePath);

    /// <summary>
    /// Gets the full URL for a file
    /// </summary>
    /// <param name="filePath">The relative file path</param>
    /// <returns>The full URL to access the file</returns>
    string GetFileUrl(string filePath);
}
