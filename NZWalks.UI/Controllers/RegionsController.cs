using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models.DTO;
using System.Threading.Tasks;

namespace NZWalks.UI.Controllers
{
  public class RegionsController : Controller
  {
    private readonly IHttpClientFactory httpClientFactory;

    public RegionsController(IHttpClientFactory httpClientFactory)
    {
      this.httpClientFactory = httpClientFactory;
    }
    public async Task<IActionResult> Index()
    {
      List<RegionDto> response = new List<RegionDto>();



      try
      {
        //Get All Regions from Web API
        var client = httpClientFactory.CreateClient();

        var httpResponseMessage = await client.GetAsync("https://localhost:7265/api/regions");

        httpResponseMessage.EnsureSuccessStatusCode();

        response.AddRange( await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());

   
      } catch (Exception)
      {
        // Log the exception

        throw;
      }

      return View(response);
    }

    [HttpGet]
    public IActionResult Add()
    {
      return View();
    }

    [HttpGet]
    public IActionResult Add()
    {
           return View();
    }
  }
}
