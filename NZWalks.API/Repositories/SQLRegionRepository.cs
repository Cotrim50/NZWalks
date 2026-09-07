using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
  public class SQLRegionRepository : IRegionRepository
  {
    private NZWalksDbContext dbContext;

    public SQLRegionRepository(NZWalksDbContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task<Region> AddAsync(Region region)
    {
      await dbContext.Regions.AddAsync(region);
      await dbContext.SaveChangesAsync();
      return region;
    }

    public async Task<Region> DeleteAsync(Guid id)
    {
      var region = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
      if (region == null)
      {
        return null;
      }
      dbContext.Regions.Remove(region);
      await dbContext.SaveChangesAsync();
      return region;
    }

    public async Task<IEnumerable<Region>> GetAllAsync()
    {
      return await dbContext.Regions.ToListAsync();
    }

    public async Task<Region> GetAsync(Guid id)
    {
      return await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Region> UpdateAsync(Guid id, Region region)
    {
      var existingRegion = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id); if (existingRegion == null)
      {
        return null;
      }
      existingRegion.Code = region.Code;
      existingRegion.Name = region.Name;
      existingRegion.RegionImageUrl = region.RegionImageUrl;

      await dbContext.SaveChangesAsync();
      return existingRegion;
    }
  }
}
