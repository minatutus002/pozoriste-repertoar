using Pozoriste.Models.Entities;

namespace Pozoriste.DataAccess.Repositories
{
    public interface ISalaRepository
    {
        Task<List<Sala>> GetAllAsync();
    }
}
