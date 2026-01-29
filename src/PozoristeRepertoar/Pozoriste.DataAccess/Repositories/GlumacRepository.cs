using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

public class GlumacRepository : IGlumacRepository
{
    private readonly PozoristeDbContext _db;

    public GlumacRepository(PozoristeDbContext db) => _db = db;

    public Task<List<Glumac>> GetAllAsync()
        => _db.Glumci.AsNoTracking().OrderBy(g => g.PunoIme).ToListAsync();

    public Task<Glumac?> GetByIdAsync(int id)
        => _db.Glumci.FirstOrDefaultAsync(g => g.GlumacId == id);

    public async Task AddAsync(Glumac glumac) => await _db.Glumci.AddAsync(glumac);

    public async Task DeleteAsync(int id)
    {
        var g = await _db.Glumci.FindAsync(id);
        if (g != null) _db.Glumci.Remove(g);
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
