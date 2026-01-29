using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

namespace Pozoriste.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GlumacController : Controller
    {
        private readonly PozoristeDbContext _db;

        public GlumacController(PozoristeDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Glumci.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(g => g.PunoIme.Contains(q));
            }

            var glumci = await query
                .OrderBy(g => g.PunoIme)
                .ToListAsync();

            ViewBag.Query = q;
            return View(glumci);
        }

        [HttpGet]
        public IActionResult Create() => View(new Glumac());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Glumac model)
        {
            await ValidateGlumacAsync(model, null);

            if (!ModelState.IsValid)
                return View(model);

            _db.Glumci.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Glumci.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Glumac model)
        {
            var entity = await _db.Glumci.FindAsync(model.GlumacId);
            if (entity == null) return NotFound();

            await ValidateGlumacAsync(model, model.GlumacId);

            if (!ModelState.IsValid)
                return View(model);

            entity.PunoIme = model.PunoIme;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Glumci.FindAsync(id);
            if (item == null) return NotFound();
            _db.Glumci.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateGlumacAsync(Glumac model, int? ignoreId)
        {
            model.PunoIme = (model.PunoIme ?? string.Empty).Trim();
            ModelState.Remove(nameof(Glumac.PunoIme));

            if (string.IsNullOrWhiteSpace(model.PunoIme))
            {
                ModelState.AddModelError(nameof(Glumac.PunoIme), "Puno ime je obavezno.");
                return;
            }

            if (model.PunoIme.Length > 120)
            {
                ModelState.AddModelError(nameof(Glumac.PunoIme), "Maksimalna duzina je 120 karaktera.");
                return;
            }

            var normalized = model.PunoIme.ToUpper();
            var idToIgnore = ignoreId ?? 0;

            var exists = await _db.Glumci.AsNoTracking()
                .AnyAsync(g => g.GlumacId != idToIgnore && g.PunoIme.ToUpper() == normalized);

            if (exists)
                ModelState.AddModelError(nameof(Glumac.PunoIme), "Glumac sa tim imenom vec postoji.");
        }
    }
}
