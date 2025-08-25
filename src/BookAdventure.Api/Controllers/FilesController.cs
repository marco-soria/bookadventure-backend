using BookAdventure.Dto.Request;
using BookAdventure.Dto.Response;
using BookAdventure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookAdventure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileStorage _fileStorage;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileStorage fileStorage, ILogger<FilesController> logger)
    {
        _fileStorage = fileStorage;
        _logger = logger;
    }

    /// <summary>
    /// Upload a file to the server
    /// </summary>
    /// <param name="request">File upload request</param>
    /// <returns>File upload response with file information</returns>
    [HttpPost("upload")]
    public async Task<ActionResult<BaseResponseGeneric<FileUploadResponseDto>>> UploadFile([FromForm] FileUploadRequestDto request)
    {
        try
        {
            var filePath = await _fileStorage.SaveFileAsync(request.File, request.Folder);
            var fileUrl = _fileStorage.GetFileUrl(filePath);

            var response = new BaseResponseGeneric<FileUploadResponseDto>
            {
                Success = true,
                Data = new FileUploadResponseDto
                {
                    FileName = request.File.FileName,
                    FilePath = filePath,
                    FileUrl = fileUrl,
                    FileSize = request.File.Length,
                    ContentType = request.File.ContentType,
                    UploadedAt = DateTime.UtcNow
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error uploading file: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Delete a file from the server
    /// </summary>
    /// <param name="filePath">The path of the file to delete</param>
    /// <param name="container">The container/folder name</param>
    /// <returns>Success response</returns>
    [HttpDelete]
    public async Task<ActionResult<BaseResponse>> DeleteFile([FromQuery] string filePath, [FromQuery] string container = "general")
    {
        try
        {
            await _fileStorage.DeleteFile(filePath, container);

            return Ok(new BaseResponse
            {
                Success = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error deleting file: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Check if a file exists
    /// </summary>
    /// <param name="filePath">The path of the file to check</param>
    /// <returns>Boolean response indicating if file exists</returns>
    [HttpGet("exists")]
    public ActionResult<BaseResponseGeneric<bool>> FileExists([FromQuery] string filePath)
    {
        try
        {
            var exists = _fileStorage.FileExists(filePath);

            return Ok(new BaseResponseGeneric<bool>
            {
                Success = true,
                Data = exists
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse
            {
                Success = false,
                ErrorMessage = $"Error checking file: {ex.Message}"
            });
        }
    }
}
