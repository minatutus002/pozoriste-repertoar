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
                .Include(p => p.Zanr)
                .ToListAsync();
        }

        public async Task<Predstava?> GetByIdAsync(int id)
        {
            return await _db.Predstave
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
    }
}
