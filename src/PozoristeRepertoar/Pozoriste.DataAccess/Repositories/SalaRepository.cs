using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Repositories
{
    public class SalaRepository : ISalaRepository
    {
        private readonly PozoristeDbContext _db;

        public SalaRepository(PozoristeDbContext db)
        {
            _db = db;
        }

        public Task<List<Sala>> GetAllAsync()
        {
            return _db.Sale
                .AsNoTracking()
                .OrderBy(s => s.Naziv)
                .ToListAsync();
        }
    }
}
