using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Repositories
{
    public class RezervacijaRepository : IRezervacijaRepository
    {
        private readonly PozoristeDbContext _db;

        public RezervacijaRepository(PozoristeDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(string userId, int terminId, int brojKarata)
        {
            var r = new Rezervacija
            {
                KorisnikId = userId,
                TerminId = terminId,
                BrojKarata = brojKarata,
                DatumKreiranja = DateTime.Now
            };

            _db.Rezervacije.Add(r);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Rezervacija>> GetByUserAsync(string userId)
        {
            return await _db.Rezervacije
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Termin)
                    .ThenInclude(t => t.Predstava)
                .Include(x => x.Termin)
                    .ThenInclude(t => t.Sala)
                .Include(x => x.Sedista)
                .Where(x => x.KorisnikId == userId)
                .OrderByDescending(x => x.DatumKreiranja)
                .ToListAsync();
        }
    }
}
