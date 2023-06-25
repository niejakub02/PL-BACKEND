using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using pl_backend.Services;
using System.Data;

namespace pl_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DataContext DataContext;
        private readonly ITokenService TokenService;
        private readonly IUserService UserService;

        public UserController(IConfiguration configuration, DataContext dataContext, ITokenService tokenService, IUserService userService)
        {
            Configuration = configuration;
            DataContext = dataContext;
            TokenService = tokenService;
            UserService = userService;
        }

        [HttpPost("SignUpUser")]
        public async Task<ActionResult<User>> SignUpUser(UserDto userDto)
        {
            try
            {
                User? user = await UserService.Authorization(userDto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SignInUser")]
        public async Task<ActionResult<string>> SignInUser(UserDto userDto)
        {
            try
            {
                string? token = await UserService.Login(userDto);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<User>> GetUser(int Id)
        {
            try
            {
                User? user = await UserService.GetUserId(Id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{Id}/invite")]
        public async Task<ActionResult<Contact>> InviteUser(int id)
        {
            try
            {
                Contact? contact = await UserService.InviteUser(id);
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{Id}/DeclineInvitation")]
        public async Task<ActionResult<Contact>> DeclineInvitation(int id)
        {
            try
            {
                Contact? contact = await UserService.DeclineInvitation(id);
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{Id}/GetReviews")]
        public async Task<ActionResult<List<GetReviewDto>>> GetReviews(int id)
        {
            try
            {
                List<GetReviewDto> reviewDtos = await UserService.GetReviews(id);
                return reviewDtos;
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("AddReview")]
        public async Task<ActionResult<Review>> AddReview(AddReviewDto addReviewDto)
        {
            try
            {
                Review? review = await UserService.AddReview(addReviewDto);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Contacts")]
        public async Task<ActionResult<List<UserContactDto>>> GetContacts()
        {
            try
            {
                List<UserContactDto> users = await UserService.GetContacts();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("Invitations")]
        public async Task<ActionResult<List<UserContactDto>>> GetInvitations()
        {
            try
            {
                List<UserContactDto> users = await UserService.GetInvitations();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("Verify")]
        public async Task<ActionResult<User>> Verify()
        {
            User user = TokenService.GetCurrentUser();
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<User>> Edit(UserUpdateDto userUpdateDto)
        {
            try
            {
                User? user = await UserService.UpdateUser(userUpdateDto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
