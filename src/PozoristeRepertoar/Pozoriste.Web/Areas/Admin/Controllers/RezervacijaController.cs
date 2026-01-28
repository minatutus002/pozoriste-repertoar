using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.Models.Entities;
using Pozoriste.Web.Models;

namespace Pozoriste.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RezervacijaController : Controller
    {
        private readonly PozoristeDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public RezervacijaController(PozoristeDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var rezervacije = await _db.Rezervacije
                .AsNoTracking()
                .Include(r => r.Termin)
                    .ThenInclude(t => t.Predstava)
                .Include(r => r.Termin)
                    .ThenInclude(t => t.Sala)
                .Include(r => r.Sedista)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();

            var userIds = rezervacije.Select(r => r.KorisnikId).Distinct().ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var emailById = users.ToDictionary(
                u => u.Id,
                u => string.IsNullOrWhiteSpace(u.Email) ? (u.UserName ?? u.Id) : u.Email!);

            var model = rezervacije.Select(r => new AdminRezervacijaRowVM
            {
                RezervacijaId = r.RezervacijaId,
                KorisnikEmail = emailById.GetValueOrDefault(r.KorisnikId, r.KorisnikId),
                Predstava = r.Termin?.Predstava?.Naziv ?? "-",
                Sala = r.Termin?.Sala?.Naziv ?? "-",
                DatumVreme = r.Termin?.DatumVreme ?? DateTime.MinValue,
                BrojKarata = r.BrojKarata,
                Cena = r.Termin?.Predstava?.Cena ?? 0m,
                Status = r.Status,
                Sedista = string.Join(", ", r.Sedista
                    .OrderBy(s => s.Red)
                    .ThenBy(s => s.Broj)
                    .Select(s => $"{GetRowLabel(s.Red)}{s.Broj}"))
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var rez = await _db.Rezervacije.FirstOrDefaultAsync(r => r.RezervacijaId == id);
            if (rez == null) return NotFound();

            if (rez.Status == RezervacijaStatus.Rezervisano)
            {
                rez.Status = RezervacijaStatus.Placeno;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCancel(int id)
        {
            var rez = await _db.Rezervacije
                .Include(r => r.Sedista)
                .FirstOrDefaultAsync(r => r.RezervacijaId == id);
            if (rez == null) return NotFound();

            if (rez.Status == RezervacijaStatus.OtkazivanjeNaCekanju ||
                rez.Status == RezervacijaStatus.OtkazivanjeNaCekanjuPlaceno)
            {
                rez.Status = rez.Status == RezervacijaStatus.OtkazivanjeNaCekanjuPlaceno
                    ? RezervacijaStatus.Refundiran
                    : RezervacijaStatus.Otkazano;

                if (rez.Sedista.Count > 0)
                    _db.RezervacijaSedista.RemoveRange(rez.Sedista);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCancel(int id)
        {
            var rez = await _db.Rezervacije.FirstOrDefaultAsync(r => r.RezervacijaId == id);
            if (rez == null) return NotFound();

            if (rez.Status == RezervacijaStatus.OtkazivanjeNaCekanju)
            {
                rez.Status = RezervacijaStatus.Rezervisano;
                await _db.SaveChangesAsync();
            }
            else if (rez.Status == RezervacijaStatus.OtkazivanjeNaCekanjuPlaceno)
            {
                rez.Status = RezervacijaStatus.Placeno;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCanceled(int id)
        {
            var rez = await _db.Rezervacije
                .Include(r => r.Sedista)
                .FirstOrDefaultAsync(r => r.RezervacijaId == id);
            if (rez == null) return NotFound();

            if (rez.Status != RezervacijaStatus.Otkazano &&
                rez.Status != RezervacijaStatus.Refundiran)
            {
                return RedirectToAction(nameof(Index));
            }

            if (rez.Sedista.Count > 0)
                _db.RezervacijaSedista.RemoveRange(rez.Sedista);

            _db.Rezervacije.Remove(rez);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private static string GetRowLabel(int row)
        {
            if (row >= 1 && row <= 26)
                return ((char)('A' + row - 1)).ToString();

            return row.ToString();
        }
    }
}
