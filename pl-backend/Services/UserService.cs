using Microsoft.AspNetCore.Mvc;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace pl_backend.Services
{
    public interface IUserService
    {
        Task<User> Authorization(UserDto userDto);
        Task<string> Login(UserDto userDto);
        Task<User> GetUserId(int Id);
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
