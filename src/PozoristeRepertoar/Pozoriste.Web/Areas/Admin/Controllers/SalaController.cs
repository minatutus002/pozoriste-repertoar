using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

namespace Pozoriste.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SalaController : Controller
    {
        private readonly PozoristeDbContext _db;
        public SalaController(PozoristeDbContext db) => _db = db;

        public async Task<IActionResult> Index()
            => View(await _db.Sale.AsNoTracking().OrderBy(s => s.Naziv).ToListAsync());

        [HttpGet]
        public IActionResult Create() => View(new Sala());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sala model)
        {
            if (!ModelState.IsValid) return View(model);
            model.Kapacitet = Math.Max(0, model.BrojRedova * model.SedistaPoRedu);
            _db.Sale.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Sale.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sala model)
        {
            if (!ModelState.IsValid) return View(model);
            model.Kapacitet = Math.Max(0, model.BrojRedova * model.SedistaPoRedu);
            _db.Sale.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Sale.FindAsync(id);
            if (item == null) return NotFound();
            _db.Sale.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
