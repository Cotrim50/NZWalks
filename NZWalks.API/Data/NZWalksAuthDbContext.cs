using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NZWalks.API.Data
{
  public class NZWalksAuthDbContext : IdentityDbContext
  {
    public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> options) : base(options)
    {
    
    }

    override protected void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      var readerRoleId = "98f28642-d2f5-4ba2-94ed-d5a6561699af";
      var writerRoleId = "442cc479-5a1e-43b0-9098-5099d8f5104e";

      var roles = new List<IdentityRole>
      {
        new IdentityRole
        {
          Id = readerRoleId,
          Name = "Reader",
          NormalizedName = "READER",
          ConcurrencyStamp = readerRoleId

        },
        new IdentityRole
        {
          Id = writerRoleId,
          Name = "Writer",
          NormalizedName = "WRITER",
          ConcurrencyStamp = writerRoleId
        }
      };
       
      builder.Entity<IdentityRole>().HasData(roles);
    }

  }
}
