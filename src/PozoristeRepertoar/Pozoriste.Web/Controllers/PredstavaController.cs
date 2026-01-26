using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.DataAccess.Repositories;
using Pozoriste.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Pozoriste.Web.Controllers
{
    public class PredstavaController : Controller
    {
        private readonly IPredstavaRepository _predstavaRepo;
        private readonly PozoristeDbContext _db;

        public PredstavaController(IPredstavaRepository predstavaRepo, PozoristeDbContext db)
        {
            _predstavaRepo = predstavaRepo;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var predstave = await _predstavaRepo.GetAllAsync();
            return View(predstave.OrderBy(p => p.Naziv).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var predstava = await _db.Predstave
                .AsNoTracking()
                .Include(p => p.Zanr)
                .FirstOrDefaultAsync(p => p.PredstavaId == id);

            if (predstava == null) return NotFound();

            var termini = await _db.Termini
                .AsNoTracking()
                .Include(t => t.Sala)
                .Where(t => t.PredstavaId == id && t.DatumVreme >= DateTime.Today)
                .OrderBy(t => t.DatumVreme)
                .ToListAsync();

            var terminIds = termini.Select(t => t.TerminId).ToList();
            var zauzeta = await _db.RezervacijaSedista
                .AsNoTracking()
                .Include(rs => rs.Rezervacija)
                .Where(rs => terminIds.Contains(rs.TerminId) &&
                             rs.Rezervacija.Status != RezervacijaStatus.Otkazano &&
                             rs.Rezervacija.Status != RezervacijaStatus.Refundiran)
                .GroupBy(rs => rs.TerminId)
                .Select(g => new { TerminId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.TerminId, v => v.Count);

            var vm = new Pozoriste.Web.Models.PredstavaDetailsVM
            {
                PredstavaId = predstava.PredstavaId,
                Naziv = predstava.Naziv,
                Zanr = predstava.Zanr?.Naziv ?? "-",
                Opis = predstava.Opis,
                SlikaUrl = predstava.SlikaUrl,
                Cena = predstava.Cena,
                TrajanjeMin = predstava.TrajanjeMin,
                Termini = termini.Select(t =>
                {
                    var reserved = zauzeta.TryGetValue(t.TerminId, out var count) ? count : 0;
                    var kapacitet = t.Sala?.Kapacitet ?? 0;
                    var slobodno = kapacitet > 0 ? Math.Max(0, kapacitet - reserved) : 0;

                    return new Pozoriste.Web.Models.PredstavaDetailsVM.TerminInfo
                    {
                        TerminId = t.TerminId,
                        DatumVreme = t.DatumVreme,
                        Sala = t.Sala?.Naziv ?? "-",
                        Kapacitet = kapacitet,
                        Slobodno = slobodno
                    };
                }).ToList()
            };

            return View(vm);
        }
    }
}
