using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using pl_backend.Services;
using System.Data;

namespace pl_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly DataContext DataContext;
        private readonly ILanguageService LanguageService;

        public LanguageController(DataContext dataContext, ILanguageService languageService)
        {
            DataContext = dataContext;
            LanguageService = languageService;
        }

        [HttpGet("Languages")]
        public async Task<ActionResult<Language>> GetLanguage()
        {
            try
            {
                var languages = await LanguageService.GetLanguages();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
