using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;
using Pozoriste.Web.Models;

namespace Pozoriste.Web.Controllers
{
    public class RepertoarController : Controller
    {
        private readonly PozoristeDbContext _db;

        public RepertoarController(PozoristeDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? zanrId, int? salaId, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = _db.Termini
                .AsNoTracking()
                .Include(t => t.Predstava)
                    .ThenInclude(p => p.Zanr)
                .Include(t => t.Sala)
                .AsQueryable();

            if (zanrId.HasValue && zanrId.Value > 0)
                query = query.Where(t => t.Predstava.ZanrId == zanrId.Value);

            if (salaId.HasValue && salaId.Value > 0)
                query = query.Where(t => t.SalaId == salaId.Value);

            if (dateFrom.HasValue)
                query = query.Where(t => t.DatumVreme.Date >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                query = query.Where(t => t.DatumVreme.Date <= dateTo.Value.Date);

            var termini = await query
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

            var items = termini.Select(t =>
            {
                var reserved = zauzeta.TryGetValue(t.TerminId, out var count) ? count : 0;
                var kapacitet = t.Sala.Kapacitet;
                var slobodno = kapacitet > 0 ? Math.Max(0, kapacitet - reserved) : 0;

                return new RepertoarRowVM
                {
                    TerminId = t.TerminId,
                    PredstavaId = t.PredstavaId,
                    Naziv = t.Predstava.Naziv,
                    Zanr = t.Predstava.Zanr.Naziv,
                    SlikaUrl = t.Predstava.SlikaUrl,
                    Cena = t.Predstava.Cena,
                    TrajanjeMin = t.Predstava.TrajanjeMin,
                    Sala = t.Sala.Naziv,
                    Kapacitet = kapacitet,
                    Slobodno = slobodno,
                    DatumVreme = t.DatumVreme
                };
            }).ToList();

            var zanrovi = await _db.Zanrovi.AsNoTracking().OrderBy(z => z.Naziv).ToListAsync();
            var sale = await _db.Sale.AsNoTracking().OrderBy(s => s.Naziv).ToListAsync();

            var vm = new RepertoarIndexVM
            {
                ZanrId = zanrId,
                SalaId = salaId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Zanrovi = zanrovi.Select(z => new SelectListItem(z.Naziv, z.ZanrId.ToString(), z.ZanrId == zanrId)).ToList(),
                Sale = sale.Select(s => new SelectListItem(s.Naziv, s.SalaId.ToString(), s.SalaId == salaId)).ToList(),
                Items = items
            };

            return View(vm);
        }
    }
}
