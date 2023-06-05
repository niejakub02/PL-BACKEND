using Microsoft.AspNetCore.Mvc;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace pl_backend.Services
{
    public interface IUserService
    {
        Task<User> Authorization(UserDto userDto);
        Task<string> Login(UserDto userDto);
        Task<User> GetUserId(int Id);
        Task<User> UpdateUser(UserUpdateDto userUpdateDto);
        Task<Contact> InviteUser(int Id);
    }
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        public UserService(DataContext dataContext, ITokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        public async Task<User> Authorization(UserDto userDto)
        {

            if (userDto.Password.Length < 8) throw new Exception("Password is too short! It must be 8 characters long!");

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new()
            {
                Email = userDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserId(int Id)
        {
            User? user = await _dataContext.Users.Where(u => u.Id == Id).FirstOrDefaultAsync();

            if (user == null) throw new Exception("User not found");

            return user;
        }

        public async Task<Contact> InviteUser(int id)
        {
            User? currentUser = _tokenService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            User? user = await _dataContext.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User you want to invite was not found");
            }

            Contact? contactAnswer = await _dataContext.Contacts.FirstOrDefaultAsync(c => c.InvitingUserId == id && c.InvitedUserId == currentUser.Id);
            if (contactAnswer != null)
            {
                if (contactAnswer.Status == false)
                {
                    contactAnswer.Status = true;
                    await _dataContext.SaveChangesAsync();
                    return contactAnswer;
                }
                else
                {
                    throw new Exception("You already met");
                }
            }


            Contact? contact = await _dataContext.Contacts.FirstOrDefaultAsync(c => c.InvitingUserId == currentUser.Id && c.InvitedUserId == id);
            if (contact == null)
            {
                Contact _contact = new()
                {
                    Status = false,
                    Met = DateTime.Now,
                    InvitingUserId = currentUser.Id,
                    InvitedUserId = id
                };
                _dataContext.Contacts.Add(_contact);
                await _dataContext.SaveChangesAsync();
                return _contact;
            }
            throw new Exception("Already sent invitation");
        }

        public async Task<string> Login(UserDto userDto)
        {
            User? user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user == null)
                throw new Exception("User not found");

            if (user.Email == null)
                throw new Exception("User not found");

            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Wrong password");

            string token = _tokenService.CreateToken(user);

            return token;
        }

        public async Task<User> UpdateUser(UserUpdateDto userUpdateDto)
        {

            User? userId = _tokenService.GetCurrentUser();
            if (userId == null)
            {
                throw new Exception("User not found");
            }

            User? user = await _dataContext.Users.FindAsync(userId.Id);
            if (user == null) throw new Exception("User not found");

            if (userUpdateDto.FirstName != null) user.FirstName = userUpdateDto.FirstName;
            if (userUpdateDto.LastName != null) user.LastName = userUpdateDto.LastName;
            if (userUpdateDto.Age != default) user.Age = userUpdateDto.Age;
            if (userUpdateDto.Description != null) user.Description = userUpdateDto.Description;

            if (userUpdateDto.Language != default)
            {
                List<UserLanguage> languages = await _dataContext.UserLanguages
                    .Where(ul => ul.UserId == user.Id)
                    .ToListAsync();

                _dataContext.UserLanguages.RemoveRange(languages);
                foreach (int languageId in userUpdateDto.Language)
                {
                    Language? language = await _dataContext.Languages.FindAsync(languageId);
                    UserLanguage userLanguage = new()
                    {
                        UserId = user.Id,
                        LanguageId = languageId
                    };

                    _dataContext.UserLanguages.Add(userLanguage);
                }

            }

            await _dataContext.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}
