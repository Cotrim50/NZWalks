namespace NZWalks.API.Models.DTO
{
  public class LoginResponseDto
  {
    public string JwtToken { get; set; }
    public DateTime Expiration { get; set; }
  }
}
