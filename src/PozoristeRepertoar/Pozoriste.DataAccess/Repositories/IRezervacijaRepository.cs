using Pozoriste.Models.Entities;
public interface IRezervacijaRepository
{
    Task CreateAsync(string userId, int terminId, int brojKarata);
    Task<List<Rezervacija>> GetByUserAsync(string userId);
}
