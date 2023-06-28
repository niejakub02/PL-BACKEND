using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using pl_backend.Data;
using pl_backend.DTO;
using pl_backend.Models;
using System.Linq;

namespace pl_backend.Services
{
    public interface IMarkerService
    {
        Task<List<MarkerDto>> GetMarkers(string city, bool? offersHelp, int[] languages);
        Task<User> GetMarkerOwner(int MarkerId);
    }

    public class MarkerService : IMarkerService
    {
        private readonly DataContext _dataContext;

        public MarkerService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<MarkerDto>> GetMarkers(string city, bool? offersHelp, int[] languages)
        {
            IQueryable<Marker> query = _dataContext.Markers;

            if (city != "*")
            {
                query = query.Where(m => m.City.Contains(city));
            }

            if (offersHelp != null)
            {
                query = query.Where(m => m.OffersHelp == offersHelp);
            }

            List<Marker> markers = await query.ToListAsync();
            List<MarkerDto> markerDtos = new List<MarkerDto>();
            foreach (Marker marker in markers) {
                User? u = null;
                if (languages.Length > 0)
                {
                    foreach (int lang in languages)
                    {
                        u = await _dataContext.Users
                       .Include(u => u.Languages)
                           .ThenInclude(l => l.Language)
                       .Where(u => u.MarkerId.Equals(marker.Id) && u.Languages.Any(l => l.Language.Id == lang))
                       .FirstOrDefaultAsync();
                        if (u != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    u = await _dataContext.Users
                        .Include(u => u.Languages)
                            .ThenInclude(l => l.Language)
                        .Where(u => u.MarkerId.Equals(marker.Id))
                        .FirstOrDefaultAsync();
                }

                if (u != null)
                {
                    markerDtos.Add(new MarkerDto(marker, u));
                }
            }
            return markerDtos;
        }

        public async Task<User> GetMarkerOwner(int MarkerId)
        {
            User? user = await _dataContext.Users
                .Include(u => u.Languages)
                    .ThenInclude(l => l.Language)
                .Where(u => u.MarkerId == MarkerId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("This marker does not have owner");
            }
            return user;
        }
    }
}
