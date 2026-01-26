using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

namespace Pozoriste.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TerminController : Controller
    {
        private readonly PozoristeDbContext _db;
        public TerminController(PozoristeDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var termini = await _db.Termini
                .AsNoTracking()
                .Include(t => t.Predstava)
                .Include(t => t.Sala)
                .OrderBy(t => t.DatumVreme)
                .ToListAsync();

            return View(termini);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillDropdowns();
            return View(new Termin { DatumVreme = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Termin model)
        {
            var predstava = await _db.Predstave.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PredstavaId == model.PredstavaId);
            if (predstava == null)
                ModelState.AddModelError(nameof(Termin.PredstavaId), "Izabrana predstava ne postoji.");
            else if (predstava.TrajanjeMin <= 0)
                ModelState.AddModelError(nameof(Termin.PredstavaId), "Trajanje predstave nije ispravno.");

            if (!ModelState.IsValid)
            {
                await FillDropdowns();
                return View(model);
            }

            if (await HasOverlapAsync(model, predstava!.TrajanjeMin))
            {
                ModelState.AddModelError(nameof(Termin.DatumVreme), "Termin se preklapa sa postojecim terminom u istoj sali.");
                await FillDropdowns();
                return View(model);
            }

            _db.Termini.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Termini.FindAsync(id);
            if (item == null) return NotFound();
            await FillDropdowns();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Termin model)
        {
            var predstava = await _db.Predstave.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PredstavaId == model.PredstavaId);
            if (predstava == null)
                ModelState.AddModelError(nameof(Termin.PredstavaId), "Izabrana predstava ne postoji.");
            else if (predstava.TrajanjeMin <= 0)
                ModelState.AddModelError(nameof(Termin.PredstavaId), "Trajanje predstave nije ispravno.");

            if (!ModelState.IsValid)
            {
                await FillDropdowns();
                return View(model);
            }

            if (await HasOverlapAsync(model, predstava!.TrajanjeMin))
            {
                ModelState.AddModelError(nameof(Termin.DatumVreme), "Termin se preklapa sa postojecim terminom u istoj sali.");
                await FillDropdowns();
                return View(model);
            }

            _db.Termini.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Termini.FindAsync(id);
            if (item == null) return NotFound();
            _db.Termini.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task FillDropdowns()
        {
            var predstave = await _db.Predstave.AsNoTracking().OrderBy(p => p.Naziv).ToListAsync();
            var sale = await _db.Sale.AsNoTracking().OrderBy(s => s.Naziv).ToListAsync();

            ViewBag.Predstave = new SelectList(predstave, "PredstavaId", "Naziv");
            ViewBag.Sale = new SelectList(sale, "SalaId", "Naziv");
        }

        private async Task<bool> HasOverlapAsync(Termin model, int trajanjeMin)
        {
            var existing = await _db.Termini
                .AsNoTracking()
                .Include(t => t.Predstava)
                .Where(t => t.SalaId == model.SalaId && t.TerminId != model.TerminId)
                .ToListAsync();

            var start = model.DatumVreme;
            var end = start.AddMinutes(trajanjeMin);

            return existing.Any(t =>
                start < t.DatumVreme.AddMinutes(t.Predstava.TrajanjeMin) &&
                end > t.DatumVreme);
        }
    }
}
