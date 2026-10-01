using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
  // https://localhost:1234/api/regions
  [Route("api/[controller]")]
  [ApiController]
  public class RegionsController : ControllerBase
  {
    private NZWalksDbContext dbContext;
    private readonly IMapper mapper;
    private readonly ILogger<RegionsController> logger;
    private readonly IRegionRepository regionRepository;

    public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper,
      ILogger<RegionsController> logger)
    {
      this.dbContext = dbContext;
      this.mapper = mapper;
      this.logger = logger;
      this.regionRepository = regionRepository;

    }


    // GET: https://localhost:1234/api/regions
    [HttpGet]
    //[Authorize(Roles ="Reader")]
    public async Task<IActionResult> GetAll()
    {
      logger.LogInformation("GetAllRegion Action Method was invoked");

      //throw new Exception("fds");
      var regions = await regionRepository.GetAllAsync();

      logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regions)}");


      return Ok(mapper.Map<List<RegionDto>>(regions));
    }

    //GET SINGLE REGION (Get Region By ID)
    [HttpGet("{id}")]
    //[Authorize(Roles = "Reader")]
    public async Task<IActionResult> Get(Guid id)
    {

      var region = await regionRepository.GetAsync(id);
      if (region == null)
      {
        return NotFound();
      }

      return Ok(mapper.Map<RegionDto>(region));
    }

    //POST to create New Region
    //POST: https://localhost:1234/api/regions
    [HttpPost]
    [ValidateModel]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
      //Map or Convert DTO to Domain Model
      var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

      //Use Domain Model to create Region
      await regionRepository.AddAsync(regionDomainModel);

      //map Domain model back to DTO
      var regionDto = mapper.Map<RegionDto>(regionDomainModel);

      return CreatedAtAction(nameof(Get), new { id = regionDto.Id }, regionDto);

    }

    //Update region
    //PUT: https://localhost:portnumber/api/regions/{id}  
    [HttpPut]
    [Route("{id:Guid}")]
    [ValidateModel]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
    {
      //Map DTO to DOmain Model
      var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

      //Check if region exists
      var updatedRegion = await regionRepository.UpdateAsync(id, regionDomainModel);

      if (updatedRegion == null)
      {
        return NotFound();
      }

      await dbContext.SaveChangesAsync();

      //Map Domain model to DTO
      var regionDto = mapper.Map<RegionDto>(regionDomainModel);
      return Ok(regionDto);

    }

    //DELETE region
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Writer")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
      //Check if region exists
      var regionDomainModel = await regionRepository.DeleteAsync(id);
      if (regionDomainModel == null)
      {
        return NotFound();
      }

      //Map Domain model to DTO
      var regionDto = mapper.Map<RegionDto>(regionDomainModel);
      return Ok(regionDto);
    }
  }
}

