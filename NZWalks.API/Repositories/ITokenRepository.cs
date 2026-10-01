using Microsoft.AspNetCore.Identity;

namespace NZWalks.API.Repositories
{
  public interface ITokenRepository
  {
    string CreateJWToken(IdentityUser user, List<string> roles);
  }
}
