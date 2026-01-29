using System.Collections.Generic;
using System.Threading.Tasks;
using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Repositories
{
    public interface IPredstavaRepository
    {
        Task<List<Predstava>> GetAllAsync();
        Task<Predstava?> GetByIdAsync(int id);

        Task<int> CreateAsync(Predstava predstava);
        Task UpdateAsync(Predstava predstava);
        Task DeleteAsync(int id);

        Task UpdateGlumciAsync(int predstavaId, List<int> glumacIds);

    }
}
