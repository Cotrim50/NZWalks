using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class WalksController : ControllerBase
  {
    private IMapper mapper;
    private readonly IWalkRepository walkRepository;

    //CONSTRUTOR
    public WalksController(IMapper mapper, IWalkRepository walkRepository)
    {
      this.mapper = mapper;
      this.walkRepository = walkRepository;
    }


    //Create
    //POST: /api/walks
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Add([FromBody] AddWalkRequestDto addWalkRequestDto)
    {

      //Convert DTO to Domain model
      var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);
      //Pass details to Repository
      walkDomainModel = await walkRepository.CreateAsync(walkDomainModel);
      //Convert Domain model back to DTO
      return Ok(mapper.Map<WalkDto>(walkDomainModel));
    }


    //GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
      [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
      [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
    {
      var walksDomainModel = await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true,
        pageNumber, pageSize);
      //Map Domain Model to DTO
      return Ok(mapper.Map<List<WalkDto>>(walksDomainModel));
    }


    //GET: (api/walks/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      var walkDomainModel = await walkRepository.GetByIdAsync(id);
      if (walkDomainModel == null)
      {
        return NotFound();
      }
      //Map Domain Model to DTO
      return Ok(mapper.Map<WalkDto>(walkDomainModel));
    }

    //UPDATE: /api/walks/{id}
    [HttpPut("{id}")]
    [ValidateModel]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
    {

        //Convert DTO to Domain model
        var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);
        //Pass details to Repository - Get Domain model in response
        walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);
        if (walkDomainModel == null)
        {
          return NotFound();
        }
        //Convert Domain model back to DTO
        return Ok(mapper.Map<WalkDto>(walkDomainModel));

    }

    //DELTE: /api/walks/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      var walkDomainModel = await walkRepository.DeleteAsync(id);
      if (walkDomainModel == null)
      {
        return NotFound();
      }
      //Convert Domain model back to DTO
      return Ok(mapper.Map<WalkDto>(walkDomainModel));
    }
  }
}
