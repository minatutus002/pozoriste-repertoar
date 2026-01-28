using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Repositories
{
    public class TerminRepository : ITerminRepository

    {
        private readonly PozoristeDbContext _context;

        public TerminRepository(PozoristeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Termin>> GetAllAsync()
        {
            return await _context.Termini
                .AsNoTracking()
                .Include(t => t.Predstava)
                .Include(t => t.Sala)
                .OrderBy(t => t.DatumVreme)
                .ToListAsync();
        }
        public async Task<List<Termin>> GetAllWithDetailsAsync()
        {
            return await _context.Termini
                .AsNoTracking()
                .Include(t => t.Predstava)
                    .ThenInclude(p => p.Zanr)
                .Include(t => t.Sala)
                .OrderBy(t => t.DatumVreme)
                .ToListAsync();
        }
        public async Task CreateAsync(int predstavaId, int salaId, DateTime datumVreme)
        {
            var termin = new Pozoriste.Models.Entities.Termin
            {
                PredstavaId = predstavaId,
                SalaId = salaId,
                DatumVreme = datumVreme
            };

            _context.Termini.Add(termin);
            await _context.SaveChangesAsync();
        }
        public async Task<Termin?> GetByIdAsync(int terminId)
        {
            return await _context.Termini.FindAsync(terminId);
        }

        public async Task UpdateAsync(int terminId, int predstavaId, int salaId, DateTime datumVreme)
        {
            var termin = await _context.Termini.FindAsync(terminId);
            if (termin == null) return;

            termin.PredstavaId = predstavaId;
            termin.SalaId = salaId;
            termin.DatumVreme = datumVreme;

            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var termin = await _context.Termini.FindAsync(id);
            if (termin == null) return;

            _context.Termini.Remove(termin);
            await _context.SaveChangesAsync();
        }
        public async Task<Termin?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Termini
                .AsNoTracking()
                .Include(t => t.Predstava)
                .Include(t => t.Sala)
                .FirstOrDefaultAsync(t => t.TerminId == id);
        }

        public async Task<Termin?> GetOverlapAsync(int salaId, DateTime start, DateTime end, int? ignoreTerminId = null)
        {
            var termini = await _context.Termini
                .AsNoTracking()
                .Include(t => t.Predstava)
                .Where(t => t.SalaId == salaId &&
                            (!ignoreTerminId.HasValue || t.TerminId != ignoreTerminId.Value))
                .ToListAsync();

            return termini.FirstOrDefault(t =>
                t.Predstava != null &&
                start < t.DatumVreme.AddMinutes(t.Predstava.TrajanjeMin) &&
                end > t.DatumVreme);
        }

    }
}
