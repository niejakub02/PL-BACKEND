using Microsoft.AspNetCore.Mvc;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Services;
using pl_backend.Services;

namespace pl_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkerController : ControllerBase
    {
        private readonly DataContext DataContext;
        private readonly IMarkerService MarkerService;
        public MarkerController(DataContext dataContext, IMarkerService languageService)
        {
            DataContext = dataContext;
            MarkerService = languageService;
        }

        [HttpGet("Markers")]
        public async Task<ActionResult<List<MarkerDto>>> GetMarkers([FromQuery(Name = "city")] string city = "*", [FromQuery(Name = "offersHelp")] bool offersHelp = false, [FromQuery(Name = "language")] string language = "*")
        {
            try
            {
                List<MarkerDto> markers = await MarkerService.GetMarkers(city, offersHelp, language);
                return Ok(markers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{MarkerId}/Owner")]
        public async Task<ActionResult<User>> GetMarkerOwner(int MarkerId)
        {
            try
            {
                User user = await MarkerService.GetMarkerOwner(MarkerId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
