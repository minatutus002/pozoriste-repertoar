using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;
using Pozoriste.Web.Models;
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
        {
            var predstave = await _db.Predstave
                .AsNoTracking()
                .Include(p => p.Zanr)
                .Include(p => p.Glumci)
                    .ThenInclude(pg => pg.Glumac)
                .OrderBy(p => p.Naziv)
                .ToListAsync();

            return View(predstave);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await FillZanroviAsync();

            var vm = new PredstavaAdminVM
            {
                Cena = 500,
                Glumci = await GetGlumciSelectListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PredstavaAdminVM model, IFormFile? slika)
        {
            if (slika != null && slika.Length > 0)
            {
                if (!TryValidateImage(slika, out var error))
                {
                    ModelState.AddModelError(nameof(PredstavaAdminVM.SlikaUrl), error);
                }
                else
                {
                    model.SlikaUrl = await SaveImageAsync(slika);
                }
            }

            if (!ModelState.IsValid)
            {
                await FillZanroviAsync();
                model.Glumci = await GetGlumciSelectListAsync(model.SelectedGlumciIds);
                return View(model);
            }

            var entity = new Predstava
            {
                Naziv = model.Naziv,
                Opis = model.Opis,
                Cena = model.Cena,
                TrajanjeMin = model.TrajanjeMin,
                ZanrId = model.ZanrId,
                SlikaUrl = model.SlikaUrl
            };

            _db.Predstave.Add(entity);
            await _db.SaveChangesAsync();

            var selectedIds = await GetValidGlumacIdsAsync(model.SelectedGlumciIds);
            await SyncGlumciAsync(entity.PredstavaId, selectedIds);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _db.Predstave.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PredstavaId == id);
            if (entity == null) return NotFound();

            var selectedIds = await _db.PredstavaGlumci
                .AsNoTracking()
                .Where(pg => pg.PredstavaId == id)
                .Select(pg => pg.GlumacId)
                .ToListAsync();

            await FillZanroviAsync();

            var vm = new PredstavaAdminVM
            {
                PredstavaId = entity.PredstavaId,
                Naziv = entity.Naziv,
                Opis = entity.Opis,
                SlikaUrl = entity.SlikaUrl,
                Cena = entity.Cena,
                TrajanjeMin = entity.TrajanjeMin,
                ZanrId = entity.ZanrId,
                SelectedGlumciIds = selectedIds,
                Glumci = await GetGlumciSelectListAsync(selectedIds)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PredstavaAdminVM model, IFormFile? slika)
        {
            var entity = await _db.Predstave.FindAsync(model.PredstavaId);
            if (entity == null) return NotFound();

            if (slika != null && slika.Length > 0)
            {
                if (!TryValidateImage(slika, out var error))
                {
                    ModelState.AddModelError(nameof(PredstavaAdminVM.SlikaUrl), error);
                }
            }

            if (!ModelState.IsValid)
            {
                await FillZanroviAsync();
                model.SlikaUrl = entity.SlikaUrl;
                model.Glumci = await GetGlumciSelectListAsync(model.SelectedGlumciIds);
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

            var selectedIds = await GetValidGlumacIdsAsync(model.SelectedGlumciIds);
            await SyncGlumciAsync(entity.PredstavaId, selectedIds);

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

        private async Task<List<SelectListItem>> GetGlumciSelectListAsync(IEnumerable<int>? selectedIds = null)
        {
            var glumci = await _db.Glumci.AsNoTracking()
                .OrderBy(g => g.PunoIme)
                .ToListAsync();

            var selected = selectedIds != null
                ? new HashSet<int>(selectedIds)
                : new HashSet<int>();

            return glumci.Select(g => new SelectListItem
            {
                Value = g.GlumacId.ToString(),
                Text = g.PunoIme,
                Selected = selected.Contains(g.GlumacId)
            }).ToList();
        }

        private async Task<List<int>> GetValidGlumacIdsAsync(IEnumerable<int>? selectedIds)
        {
            var distinctIds = selectedIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            if (distinctIds.Count == 0) return new List<int>();

            return await _db.Glumci.AsNoTracking()
                .Where(g => distinctIds.Contains(g.GlumacId))
                .Select(g => g.GlumacId)
                .ToListAsync();
        }

        private async Task SyncGlumciAsync(int predstavaId, List<int> selectedIds)
        {
            var existing = await _db.PredstavaGlumci
                .Where(pg => pg.PredstavaId == predstavaId)
                .ToListAsync();

            var toRemove = existing.Where(pg => !selectedIds.Contains(pg.GlumacId)).ToList();
            if (toRemove.Count > 0)
                _db.PredstavaGlumci.RemoveRange(toRemove);

            var existingIds = existing.Select(pg => pg.GlumacId).ToHashSet();
            var toAdd = selectedIds.Where(id => !existingIds.Contains(id)).ToList();

            foreach (var gid in toAdd)
            {
                _db.PredstavaGlumci.Add(new PredstavaGlumac
                {
                    PredstavaId = predstavaId,
                    GlumacId = gid
                });
            }
        }
    }
}
