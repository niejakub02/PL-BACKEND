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
    public interface IContactService
    {
        Task<User> AddUser(UserContactDto userContactDto);
    }

    public class ContactService : IContactService
    {
        private readonly DataContext _dataContext;
        private readonly IContactService _tokenService;

        public ContactService(DataContext dataContext, IContactService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        public async Task<Contact> AddUser(UserContactDto userContactDto, Exception notImplementedException)
        {
            throw notImplementedException;
        }
    }
}
