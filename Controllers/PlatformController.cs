using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    /*
     * [controller] -> will be automatically replace with name "Platform".
     *              -> It is a wild card approach.
     */
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo _repositery;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        /*
         * This 2 argument repositery and mapper will be added by
         * dependecy injection.
         */
        public PlatformController(  IPlatformRepo repositery, 
                                    IMapper mapper,
                                    ICommandDataClient commandDataClient)
        {
            _repositery = repositery;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Plaforms");

            IEnumerable<Platform> platformItems = _repositery.GetAllPlatforms();

            /*
             * Here our automapper will map Platform -> PlatformReadDto.
             * We have aleady created Profiler for automapper to do so.
             */
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting Plaform by Id");

            Platform platformItem = _repositery.GetPlatformById(id);

            if(platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> Creating platform..");

            Platform platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repositery.CreatePlatform(platformModel);
            _repositery.SaveChanges(); // Don't forget to add this line.

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[EXCPETION] - {ex}");
            }
            

            /*
             * In post method after successful creation of new resource we have to 
             * retun 201 status back to the client along with resource URL and
             * resource it self. This is the standard practice.
             * 
             * Here, nameof(GetPlatformById) -> Is from GetPlatformById()
             *                               -> This is the Url for that newly created resource.
             * platformReadDto is newly created resouce.
             */
            return CreatedAtRoute(nameof(GetPlatformById),
                                    new { Id = platformReadDto.Id },
                                    platformReadDto);
        }
    }
}
