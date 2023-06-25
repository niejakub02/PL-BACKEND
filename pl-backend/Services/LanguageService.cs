using Microsoft.AspNetCore.Mvc;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Services;

namespace pl_backend.Services
{
    public interface ILanguageService
    {
        Task<List<Language>> GetLanguages();
    }

    public class LanguageService : ILanguageService
    {
        private readonly DataContext _dataContext;

        public LanguageService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Language>> GetLanguages()
        {
            List<Language> languages = await _dataContext.Languages.ToListAsync();
            return languages;
        }
    }
}
