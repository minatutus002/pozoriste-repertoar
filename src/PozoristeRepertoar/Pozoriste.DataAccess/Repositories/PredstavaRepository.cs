using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pozoriste.DataAccess.Repositories
{
    public class PredstavaRepository : IPredstavaRepository
    {
        private readonly PozoristeDbContext _db;

        public PredstavaRepository(PozoristeDbContext db)
        {
            _db = db;
        }

        public async Task<List<Predstava>> GetAllAsync()
        {
            return await _db.Predstave
                .AsNoTracking()
                .Include(p => p.Zanr)
                .OrderBy(p => p.Naziv)
                .ToListAsync();
        }

        public async Task<Predstava?> GetByIdAsync(int id)
        {
            return await _db.Predstave
                .AsNoTracking()
                .Include(p => p.Zanr)
                .FirstOrDefaultAsync(p => p.PredstavaId == id);
        }

        public async Task<int> CreateAsync(Predstava predstava)
        {
            _db.Predstave.Add(predstava);
            await _db.SaveChangesAsync();
            return predstava.PredstavaId;
        }

        public async Task UpdateAsync(Predstava predstava)
        {
            _db.Predstave.Update(predstava);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Predstave.FirstOrDefaultAsync(p => p.PredstavaId == id);
            if (entity == null) return;

            _db.Predstave.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public Task<List<int>> GetGlumacIdsAsync(int predstavaId)
        {
            return _db.PredstavaGlumci
                .AsNoTracking()
                .Where(pg => pg.PredstavaId == predstavaId)
                .Select(pg => pg.GlumacId)
                .ToListAsync();
        }

        public async Task UpdateGlumciAsync(int predstavaId, List<int> glumacIds)
        {
            var predstava = await _db.Predstave
                .Include(p => p.Glumci)
                .FirstOrDefaultAsync(p => p.PredstavaId == predstavaId);

            if (predstava == null) return;

            // ukloni stare
            predstava.Glumci.Clear();

            // dodaj nove (distinct da ne duplira)
            foreach (var gid in glumacIds.Distinct())
            {
                predstava.Glumci.Add(new PredstavaGlumac
                {
                    PredstavaId = predstavaId,
                    GlumacId = gid
                });
            }

            await _db.SaveChangesAsync();
        }

    }
}
