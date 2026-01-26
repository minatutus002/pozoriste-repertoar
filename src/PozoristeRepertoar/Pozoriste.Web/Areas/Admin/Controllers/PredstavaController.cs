using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;
using System.IO;

namespace Pozoriste.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PredstavaController : Controller
    {
        private readonly PozoristeDbContext _db;
        private readonly IWebHostEnvironment _env;

        private const long MaxImageSizeBytes = 2 * 1024 * 1024;
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public PredstavaController(PozoristeDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
            => View(await _db.Predstave
                .AsNoTracking()
                .Include(p => p.Zanr)
                .OrderBy(p => p.Naziv)
                .ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillZanroviAsync();
            return View(new Predstava { Cena = 500 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Predstava model, IFormFile? slika)
        {
            if (slika != null && slika.Length > 0)
            {
                if (!TryValidateImage(slika, out var error))
                {
                    ModelState.AddModelError(nameof(Predstava.SlikaUrl), error);
                }
                else
                {
                    model.SlikaUrl = await SaveImageAsync(slika);
                }
            }

            if (!ModelState.IsValid)
            {
                await FillZanroviAsync();
                return View(model);
            }

            _db.Predstave.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Predstave.FindAsync(id);
            if (item == null) return NotFound();
            await FillZanroviAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Predstava model, IFormFile? slika)
        {
            var entity = await _db.Predstave.FindAsync(model.PredstavaId);
            if (entity == null) return NotFound();

            if (slika != null && slika.Length > 0)
            {
                if (!TryValidateImage(slika, out var error))
                {
                    ModelState.AddModelError(nameof(Predstava.SlikaUrl), error);
                }
            }

            if (!ModelState.IsValid)
            {
                await FillZanroviAsync();
                model.SlikaUrl = entity.SlikaUrl;
                return View(model);
            }

            entity.Naziv = model.Naziv;
            entity.Opis = model.Opis;
            entity.Cena = model.Cena;
            entity.TrajanjeMin = model.TrajanjeMin;
            entity.ZanrId = model.ZanrId;

            if (slika != null && slika.Length > 0)
            {
                TryDeleteImage(entity.SlikaUrl);
                entity.SlikaUrl = await SaveImageAsync(slika);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Predstave.FindAsync(id);
            if (item == null) return NotFound();
            var imageUrl = item.SlikaUrl;
            _db.Predstave.Remove(item);
            await _db.SaveChangesAsync();
            TryDeleteImage(imageUrl);
            return RedirectToAction(nameof(Index));
        }

        private bool TryValidateImage(IFormFile file, out string error)
        {
            if (file.Length == 0)
            {
                error = "Izabrani fajl je prazan.";
                return false;
            }

            if (file.Length > MaxImageSizeBytes)
            {
                error = "Maksimalna velicina slike je 2MB.";
                return false;
            }

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            {
                error = "Dozvoljeni formati su JPG, PNG i WEBP.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";

            var root = Path.Combine(_env.WebRootPath, "uploads", "predstave");
            Directory.CreateDirectory(root);

            var path = Path.Combine(root, fileName);
            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/predstave/{fileName}";
        }

        private void TryDeleteImage(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            var fileName = Path.GetFileName(url);
            if (string.IsNullOrWhiteSpace(fileName)) return;

            var path = Path.Combine(_env.WebRootPath, "uploads", "predstave", fileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        private async Task FillZanroviAsync()
        {
            var zanrovi = await _db.Zanrovi.AsNoTracking().OrderBy(z => z.Naziv).ToListAsync();
            ViewBag.Zanrovi = new SelectList(zanrovi, "ZanrId", "Naziv");
        }
    }
}
