using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
  public class LocalImageRepository : IImageRepository
  {
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly NZWalksDbContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, NZWalksDbContext dbContext)
    {
      this.webHostEnvironment = webHostEnvironment;
      this.httpContextAccessor = httpContextAccessor;
      this.dbContext = dbContext;
    }


    public async Task<Image> Upload(Image image)
    {
      var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");

      //Uplaod iamge to local path
      using var stream = new FileStream(localFilePath, FileMode.Create);
      await image.File.CopyToAsync(stream);

      // https://localhost:1234/images/image.jpg

      var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

      image.FilePath = urlFilePath;

      // Add Image to the Images table
      await dbContext.Images.AddAsync(image);
      await dbContext.SaveChangesAsync();

      return image;

    }
  }
}
