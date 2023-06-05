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
    public class ContactController : ControllerBase
    {
        private readonly DataContext DataContext;
        private readonly ITokenService TokenService;
        private readonly IContactService ContactService;

        public ContactController(DataContext dataContext, ITokenService tokenService, IContactService contactService)
        {
            DataContext = dataContext;
            TokenService = tokenService;
            ContactService = contactService;
        }
    }
}
