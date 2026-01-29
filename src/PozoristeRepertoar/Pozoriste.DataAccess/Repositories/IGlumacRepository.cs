using Pozoriste.Models.Entities;

public interface IGlumacRepository
{
    Task<List<Glumac>> GetAllAsync();
    Task<Glumac?> GetByIdAsync(int id);
    Task AddAsync(Glumac glumac);
    Task DeleteAsync(int id);
    Task SaveAsync();
}
