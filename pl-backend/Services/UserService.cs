using Microsoft.AspNetCore.Mvc;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Hosting;

namespace pl_backend.Services
{
    public interface IUserService
    {
        Task<User> Authorization(UserDto userDto);
        Task<string> Login(UserDto userDto);
        Task<User> GetUserId(int Id);
        Task<User> UpdateUser(UserUpdateDto userUpdateDto, IFormFile? imageFile);
        Task<Contact> InviteUser(int Id);
        Task<Contact> DeclineInvitation(int id);
        Task<List<UserContactDto>> GetContacts();
        Task<List<UserContactDto>> GetInvitations();
        Task<Review> AddReview(AddReviewDto addReviewDto);
        Task<List<GetReviewDto>> GetReviews(int id);
        Task<Marker> AddMarker(Marker marker);
        Task<User> DeleteMarker();
        Task<User> Informations();
    }
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserService(DataContext dataContext, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
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
            User? user = await _dataContext.Users
                .Include(u => u.Contacts)
                .Include(u => u.Languages)
                .Include(u => u.Marker)
                .Where(u => u.Id == Id)
                .FirstOrDefaultAsync();

            if (user == null) throw new Exception("User not found");

            return user;
        }

        public async Task<Marker> AddMarker(Marker marker)
        {
            User? user = await _dataContext.Users.FindAsync(_tokenService.GetCurrentUser()?.Id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.MarkerId != null)
            {
                Marker? newMarker = await _dataContext.Markers.FindAsync(user.MarkerId);
                newMarker.Latitude = marker.Latitude;
                newMarker.Longitude = marker.Longitude;
                newMarker.City= marker.City;
                newMarker.OffersHelp = marker.OffersHelp;
                await _dataContext.SaveChangesAsync();
            }
            else
            {

                Marker newMarker= new Marker()
                {
                    Latitude = marker.Latitude,
                    Longitude = marker.Longitude,
                    City = marker.City,
                    OffersHelp = marker.OffersHelp,
                };
                _dataContext.Markers.Add(newMarker);
                await _dataContext.SaveChangesAsync();
                user.MarkerId = newMarker.Id;
                await _dataContext.SaveChangesAsync();
            }

            return marker;
        }

        public async Task<User> DeleteMarker()
        {
            User? currentUser = _tokenService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            User? user = await _dataContext.Users.FindAsync(currentUser.Id);
            if (user == null)
            {
                throw new Exception("User not found - can not delete marker");
            }

            if (user.MarkerId != null)
            {
                Marker? marker = await _dataContext.Markers.FindAsync(user.MarkerId);
                if (marker != null)
                {
                    _dataContext.Markers.Remove(marker);
                }
            }

            user.MarkerId = null;
            user.Marker = null;

            await _dataContext.SaveChangesAsync();

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
                    Met = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                    InvitingUserId = currentUser.Id,
                    InvitedUserId = id
                };
                _dataContext.Contacts.Add(_contact);
                await _dataContext.SaveChangesAsync();
                return _contact;
            }
            throw new Exception("Already sent invitation");
        }

        public async Task<Contact> DeclineInvitation(int id)
        {
            User? currentUser = _tokenService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            User? user = await _dataContext.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("You can not deciline invitation to user that does not exist");
            }

            Contact? invitation = await _dataContext.Contacts.FirstOrDefaultAsync(c => (c.InvitingUserId == id || c.InvitingUserId == currentUser.Id) && (c.InvitedUserId == currentUser.Id || c.InvitedUserId == id));
            if (invitation == null)
            {
                throw new Exception("There is no such invitation");
            }
            _dataContext.Contacts.Remove(invitation);
            await _dataContext.SaveChangesAsync();
            return invitation;
        }

        public async Task<User> Informations()
        {
            User? currentUser = _tokenService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            User? user = await _dataContext.Users
                .Include(u => u.Contacts)
                .Include(u => u.Marker)
                .Include(u => u.Languages)
                    .ThenInclude(l => l.Language)
                .Include(u => u.Chats)
                .Where(u => u.Id == currentUser.Id)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("You can not deciline invitation to user that does not exist");
            }
            return user;
        }

        public async Task<List<UserContactDto>> GetContacts()
        {
            User? currentUser = _tokenService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }
            var selectedContacts = await _dataContext.Contacts
                .Where(c => (c.InvitingUserId == currentUser.Id || c.InvitedUserId == currentUser.Id) && c.Status.Equals(true))
                .ToListAsync();

            var selectedInvitingContactsId = selectedContacts
                .Where(c => c.InvitingUserId != currentUser.Id)
                .Select(c => c.InvitingUserId);

            var selectedInvitedContactsId = selectedContacts
                .Where(c => c.InvitedUserId != currentUser.Id)
                .Select(c => c.InvitedUserId);

            List<User> users = await _dataContext.Users
                .Where(u => selectedInvitingContactsId.Contains(u.Id) || selectedInvitedContactsId.Contains(u.Id))
                .ToListAsync();

            var userContacts = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserContactDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));
            });
            var mapper = userContacts.CreateMapper();
            List<UserContactDto> userContactDtos = mapper.Map<List<UserContactDto>>(users);
            if (userContactDtos == null)
            {
                throw new Exception("No contacts");
            }
            return userContactDtos;
        }

        public async Task<List<UserContactDto>> GetInvitations()
        {
            User? currentUser = _tokenService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }
            var selectedContacts = await _dataContext.Contacts
                .Where(c => (c.InvitingUserId == currentUser.Id || c.InvitedUserId == currentUser.Id) && c.Status.Equals(false))
                .ToListAsync();

            var selectedInvitingContactsId = selectedContacts
                .Where(c => c.InvitingUserId != currentUser.Id)
                .Select(c => c.InvitingUserId);

            var selectedInvitedContactsId = selectedContacts
                .Where(c => c.InvitedUserId != currentUser.Id)
                .Select(c => c.InvitedUserId);

            List<User> users = await _dataContext.Users
                .Where(u => selectedInvitingContactsId.Contains(u.Id) || selectedInvitedContactsId.Contains(u.Id))
                .ToListAsync();

            var userContacts = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserContactDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));
            });
            var mapper = userContacts.CreateMapper();
            List<UserContactDto> userContactDtos = mapper.Map<List<UserContactDto>>(users);
            if (userContactDtos == null)
            {
                throw new Exception("No contacts");
            }
            return userContactDtos;
        }

        public async Task<Review> AddReview(AddReviewDto addReviewDto)
        {
            User? currentUser = _tokenService.GetCurrentUser();
            User? currentUserDb = await _dataContext.Users.FindAsync(currentUser.Id);
            if (currentUserDb == null)
            {
                throw new Exception("User not found");
            }

            bool isRequestGood = true;
            if (addReviewDto.ToId == default) isRequestGood = false;
            if (addReviewDto.Rating == default) isRequestGood = false;

            if (!isRequestGood)
            {
                throw new Exception("Bad request, check if you provided all of the needed informations");
            }

            User? user = await _dataContext.Users.FindAsync(addReviewDto.ToId);
            if (user == null)
            {
                throw new Exception("You can add review to user that does not exist");
            }

            List<UserContactDto> userContactDtos = await GetContacts();
            bool isContact = false;
            foreach (UserContactDto contact in userContactDtos)
            {
                if (contact.Id == addReviewDto.ToId)
                {
                    isContact = true;
                }
            }


            if (!isContact)
            {
                throw new Exception("You can not add review for a user that is not on your friend list");
            }

            Review? existingReview = await _dataContext.Reviews
                .Where(r => r.FromId == currentUserDb.Id && r.ToId == addReviewDto.ToId)
                .FirstOrDefaultAsync();

            Review? tmpReview = null;
            if (existingReview != null)
            {
                existingReview.Rating = addReviewDto.Rating;
                existingReview.Description = addReviewDto.Description;
                tmpReview = existingReview;
            }
            else
            {
                Review _review = new()
                {
                    Rating = addReviewDto.Rating,
                    Description = addReviewDto.Description,
                    ToId = addReviewDto.ToId,
                    FromId = currentUserDb.Id
                };

                _dataContext.Reviews.Add(_review);
                tmpReview = _review;
            }

            await _dataContext.SaveChangesAsync();
            return tmpReview;
        }

        public async Task<List<GetReviewDto>> GetReviews(int id)
        {
            User? currentUser = _tokenService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not found");
            }

            List<Review> reviews = await _dataContext.Reviews
            .Include(r => r.From)
            .Where(u => u.ToId == id)
            .ToListAsync();

            var userContacts = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Review, GetReviewDto>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.From.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.From.LastName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.From.Avatar));
            });
            var mapper = userContacts.CreateMapper();
            List<GetReviewDto> userReviewsDto = mapper.Map<List<GetReviewDto>>(reviews);
            if (userReviewsDto == null)
            {
                throw new Exception("No reviews");
            }
            return userReviewsDto;
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

        public async Task<User> UpdateUser(UserUpdateDto userUpdateDto, IFormFile? imageFile)
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

            if (imageFile != null)
            {
                string imagePath = await UploadImage(imageFile);
                user.Avatar = imagePath;
            }

            await _dataContext.SaveChangesAsync();

            return user;
        }

        private async Task<string> UploadImage(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/" + uniqueFileName;
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
