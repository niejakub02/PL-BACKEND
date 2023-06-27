using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using pl_backend.Data;
using pl_backend.Models;

namespace pl_backend.Services
{
    public interface IMarkerService
    {
        Task<List<Marker>> GetMarkers(string city, bool offersHelp);
        Task<User> GetMarkerOwner(int MarkerId);
    }

    public class MarkerService : IMarkerService
    {
        private readonly DataContext _dataContext;

        public MarkerService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Marker>> GetMarkers(string city, bool offersHelp)
        {
            IQueryable<Marker> query = _dataContext.Markers;

            if (city != "*")
            {
                query = query.Where(m => m.City.Contains(city));
            }

            if (offersHelp)
            {
                query = query.Where(m => m.OffersHelp);
            }

            List<Marker> markers = await query.ToListAsync();

            //List<Marker> markers = await query.
            //    .Join(_dataContext)
            //    ToListAsync();
            //return markers;
            //List<Marker> markers = await _dataContext.Markers
            //    .Where(m => (m.City.Contains(city) && m.OffersHelp.Equals(offersHelp)))
            //    .ToListAsync();
            return markers;
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
