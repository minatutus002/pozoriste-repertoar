using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Repositories
{
    public interface IRezervacijaRepository
    {
        Task CreateAsync(string userId, int terminId, int brojKarata);
        Task<List<Rezervacija>> GetByUserAsync(string userId);
    }
}
