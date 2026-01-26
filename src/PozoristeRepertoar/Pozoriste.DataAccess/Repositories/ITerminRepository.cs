using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Repositories
{
    public interface ITerminRepository
    {
        Task<List<Termin>> GetAllAsync();
        Task<List<Termin>> GetAllWithDetailsAsync();
        Task CreateAsync(int predstavaId, int salaId, DateTime datumVreme);
        Task<Termin?> GetByIdAsync(int terminId);
        Task UpdateAsync(int terminId, int predstavaId, int salaId, DateTime datumVreme);
        Task DeleteAsync(int id);
        Task<Termin?> GetByIdWithDetailsAsync(int id);

    }
}
