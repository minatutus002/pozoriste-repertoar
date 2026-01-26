using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;

namespace Pozoriste.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ZanrController : Controller
    {
        private readonly PozoristeDbContext _db;
        public ZanrController(PozoristeDbContext db) => _db = db;

        public async Task<IActionResult> Index()
            => View(await _db.Zanrovi.AsNoTracking().OrderBy(z => z.Naziv).ToListAsync());

        [HttpGet]
        public IActionResult Create() => View(new Zanr());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Zanr model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Zanrovi.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Zanrovi.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Zanr model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Zanrovi.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Zanrovi.FindAsync(id);
            if (item == null) return NotFound();
            _db.Zanrovi.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
