using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ImagesController : ControllerBase
  {
    private readonly IImageRepository imageRepository;
    public ImagesController(IImageRepository imageRepository)
    {
      this.imageRepository = imageRepository;
    }

    // POST: /api/Images/Upload
    [HttpPost]
    [Route("Upload")]
    public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
    {
      ValidateFileUplaod(request);

      if (ModelState.IsValid)
      {
        // convert DTO to Domain model
        var imageDomainModel = new Image
        {
          File = request.File,
          FileExtension = Path.GetExtension(request.File.FileName),
          FileName = request.FileName,
          FileDescription = request.FileDescription,
          FileSizeInBytes = request.File.Length
        };

        await imageRepository.Upload(imageDomainModel);
        return Ok(imageDomainModel);
      }

      return BadRequest(ModelState);

    }

    private void ValidateFileUplaod(ImageUploadRequestDto request)
    {
      var allowedEnstensions = new string[] { ".jpg", ".jpeg", ".png" };

      if (!allowedEnstensions.Contains(Path.GetExtension(request.File.FileName)))
      {
        // Invalid file extension
        ModelState.AddModelError("File", "Unsupported file extension");
      }

      if (request.File.Length > 10485760) // 10 MB
      {
        ModelState.AddModelError("File", "File size exceeds the limit of 10 MB");
      }
    }
  }
}

