using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models;

namespace NZWalks.API.Data
{
  public class NZWalksDbContext : DbContext
  {
    public NZWalksDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

      }
      public DbSet<Region> Regions { get; set; }
      public DbSet<Difficulty> Difficulties { get; set; }
      public DbSet<Walk> Walks { get; set; }
    }
}
