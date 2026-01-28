using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.DataAccess.Repositories;
using Pozoriste.Models.Entities;
using Pozoriste.Web.Models;

namespace Pozoriste.Web.Controllers
{
    [Authorize]
    public class RezervacijaController : Controller
    {
        private readonly PozoristeDbContext _db;
        private readonly ITerminRepository _terminRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public RezervacijaController(
            PozoristeDbContext db,
            ITerminRepository terminRepo,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _terminRepo = terminRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var rezervacije = await _db.Rezervacije
                .AsNoTracking()
                .Where(r => r.KorisnikId == user.Id)
                .Include(r => r.Termin).ThenInclude(t => t.Predstava)
                .Include(r => r.Termin).ThenInclude(t => t.Sala)
                .Include(r => r.Sedista)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();

            return View(rezervacije);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int terminId)
        {
            var vm = await BuildSeatVmAsync(terminId, selected: null);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RezervacijaSeatVM vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var termin = await _terminRepo.GetByIdWithDetailsAsync(vm.TerminId);
            if (termin == null) return NotFound();

            var selected = vm.Selected ?? new List<string>();
            if (selected.Count == 0)
                ModelState.AddModelError(nameof(vm.Selected), "Morate izabrati bar jedno sediste.");

            var zauzeta = await GetZauzetaSedistaAsync(vm.TerminId);

            var parsed = new List<(int Row, int Seat)>();
            foreach (var item in selected.Distinct())
            {
                if (!TryParseSeat(item, out var row, out var seat))
                {
                    ModelState.AddModelError(nameof(vm.Selected), "Neispravan format sedista.");
                    continue;
                }

                if (row < 1 || row > termin.Sala.BrojRedova ||
                    seat < 1 || seat > termin.Sala.SedistaPoRedu)
                {
                    ModelState.AddModelError(nameof(vm.Selected), "Izabrano sediste ne postoji u sali.");
                    continue;
                }

                if (zauzeta.Contains(item))
                {
                    ModelState.AddModelError(nameof(vm.Selected), "Neko od sedista je vec zauzeto.");
                    continue;
                }

                parsed.Add((row, seat));
            }

            if (!ModelState.IsValid)
            {
                var retry = await BuildSeatVmAsync(vm.TerminId, selected);
                if (retry == null) return NotFound();
                return View(retry);
            }

            // Zone + cene
            var zones = BuildSeatZones(termin.Sala.SalaId, termin.Sala.BrojRedova);

            decimal ukupnaCena = 0m;

            var rez = new Rezervacija
            {
                TerminId = vm.TerminId,
                KorisnikId = user.Id,
                BrojKarata = parsed.Count,
                DatumKreiranja = DateTime.Now,
                Status = RezervacijaStatus.Rezervisano,
                UkupnaCena = 0m
            };

            // Sedista vezujemo preko navigacije (EF ce sam popuniti FK)
            var sedista = parsed.Select(p =>
            {
                var zona = zones.FirstOrDefault(z => p.Row >= z.OdReda && p.Row <= z.DoReda);
                var multiplier = zona?.CenaMultiplier ?? 1.0m;

                var cenaSedista = decimal.Round(termin.Predstava.Cena * multiplier, 2);
                ukupnaCena += cenaSedista;

                return new RezervacijaSediste
                {
                    TerminId = vm.TerminId,
                    Red = p.Row,
                    Broj = p.Seat,
                    Cena = cenaSedista,
                    Zona = zona?.Naziv ?? "Standard"
                };
            }).ToList();

            rez.UkupnaCena = decimal.Round(ukupnaCena, 2);
            rez.Sedista = sedista;

            _db.Rezervacije.Add(rez);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(vm.Selected),
                    "Izabrana sedista su upravo zauzeta. Pokusajte ponovo.");

                var retry = await BuildSeatVmAsync(vm.TerminId, selected);
                if (retry == null) return NotFound();
                return View(retry);
            }

            return RedirectToAction(nameof(Checkout), new { id = rez.RezervacijaId });
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var rez = await _db.Rezervacije
                .AsNoTracking()
                .Include(r => r.Termin).ThenInclude(t => t.Predstava)
                .Include(r => r.Termin).ThenInclude(t => t.Sala)
                .Include(r => r.Sedista)
                .FirstOrDefaultAsync(r => r.RezervacijaId == id && r.KorisnikId == user.Id);

            if (rez == null) return NotFound();

            var ukupno = decimal.Round(rez.UkupnaCena, 2);

            // grupisanje po zoni + ceni (jer zona moze imati istu cenu, ali i da bude sigurno)
            var stavke = rez.Sedista
                .GroupBy(s => new { s.Zona, s.Cena })
                .Select(g => new PriceLineVm
                {
                    Zona = string.IsNullOrWhiteSpace(g.Key.Zona) ? "Standard" : g.Key.Zona,
                    CenaJed = decimal.Round(g.Key.Cena, 2),
                    Kolicina = g.Count(),
                    Ukupno = decimal.Round(g.Sum(x => x.Cena), 2)
                })
                .OrderByDescending(x => x.CenaJed) // VIP gore, pa premium, pa standard (približno)
                .ToList();

            var vm = new RezervacijaCheckoutVM
            {
                RezervacijaId = rez.RezervacijaId,
                Predstava = rez.Termin?.Predstava?.Naziv ?? "-",
                Sala = rez.Termin?.Sala?.Naziv ?? "-",
                DatumVreme = rez.Termin?.DatumVreme ?? DateTime.MinValue,
                BrojKarata = rez.BrojKarata,
                Ukupno = ukupno,
                Status = rez.Status,
                Stavke = stavke,
                Sedista = string.Join(", ", rez.Sedista
                    .OrderBy(s => s.Red)
                    .ThenBy(s => s.Broj)
                    .Select(s => $"{GetRowLabel(s.Red)}{s.Broj}"))
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var rez = await _db.Rezervacije
                .FirstOrDefaultAsync(r => r.RezervacijaId == id && r.KorisnikId == user.Id);

            if (rez == null) return NotFound();

            if (rez.Status == RezervacijaStatus.Rezervisano)
            {
                rez.Status = RezervacijaStatus.Placeno;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Checkout), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestCancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var rez = await _db.Rezervacije
                .FirstOrDefaultAsync(r => r.RezervacijaId == id && r.KorisnikId == user.Id);

            if (rez == null) return NotFound();

            if (rez.Status == RezervacijaStatus.Rezervisano)
                rez.Status = RezervacijaStatus.OtkazivanjeNaCekanju;
            else if (rez.Status == RezervacijaStatus.Placeno)
                rez.Status = RezervacijaStatus.OtkazivanjeNaCekanjuPlaceno;
            else
                return RedirectToAction(nameof(Index));

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<RezervacijaSeatVM?> BuildSeatVmAsync(int terminId, IEnumerable<string>? selected)
        {
            var termin = await _terminRepo.GetByIdWithDetailsAsync(terminId);
            if (termin == null) return null;

            var zauzeta = await GetZauzetaSedistaAsync(terminId);

            var vm = new RezervacijaSeatVM
            {
                TerminId = termin.TerminId,
                Predstava = termin.Predstava.Naziv,
                Sala = termin.Sala.Naziv,
                DatumVreme = termin.DatumVreme,
                Cena = termin.Predstava.Cena, // bazna cena
                BrojRedova = termin.Sala.BrojRedova,
                SedistaPoRedu = termin.Sala.SedistaPoRedu,
                Zone = BuildSeatZones(termin.Sala.SalaId, termin.Sala.BrojRedova),
                Zauzeta = zauzeta,
                Selected = selected?.ToList() ?? new List<string>()
            };

            return vm;
        }

        private async Task<HashSet<string>> GetZauzetaSedistaAsync(int terminId)
        {
            var zauzeta = await _db.RezervacijaSedista
                .AsNoTracking()
                .Include(s => s.Rezervacija)
                .Where(s => s.TerminId == terminId &&
                            s.Rezervacija.Status != RezervacijaStatus.Otkazano &&
                            s.Rezervacija.Status != RezervacijaStatus.Refundiran)
                .Select(s => new { s.Red, s.Broj })
                .ToListAsync();

            return zauzeta.Select(s => $"{s.Red}-{s.Broj}").ToHashSet();
        }

        private static bool TryParseSeat(string value, out int row, out int seat)
        {
            row = 0;
            seat = 0;

            var parts = value.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return false;

            return int.TryParse(parts[0], out row) && int.TryParse(parts[1], out seat);
        }

        private static string GetRowLabel(int row)
        {
            if (row >= 1 && row <= 26)
                return ((char)('A' + row - 1)).ToString();

            return row.ToString();
        }

        private static List<SeatZoneVm> BuildSeatZones(int salaId, int brojRedova)
        {
            var zones = new List<SeatZoneVm>();

            void AddZone(string naziv, int from, int to, decimal multiplier, string css)
            {
                if (from > to || from < 1 || to < 1) return;

                zones.Add(new SeatZoneVm
                {
                    Naziv = naziv,
                    OdReda = from,
                    DoReda = to,
                    CenaMultiplier = multiplier,
                    CssClass = css
                });
            }

            if (brojRedova <= 0)
                return zones;

            if (salaId == 1)
            {
                AddZone("VIP", 1, 2, 1.5m, "zone-vip");
                AddZone("Premium", 3, Math.Min(6, brojRedova), 1.2m, "zone-premium");
                if (brojRedova >= 10)
                    AddZone("Balkon", Math.Max(brojRedova - 3, 7), brojRedova, 0.8m, "zone-balkon");
                return zones;
            }

            if (salaId == 2)
            {
                AddZone("VIP", 1, 2, 1.5m, "zone-vip");

                if (brojRedova >= 6)
                {
                    AddZone("Standard", 3, brojRedova - 2, 1.0m, "zone-standard");
                    AddZone("Intimna", brojRedova - 1, brojRedova, 0.9m, "zone-intimna");
                }
                else
                {
                    AddZone("Standard", 3, brojRedova, 1.0m, "zone-standard");
                }

                return zones;
            }

            AddZone("VIP", 1, Math.Min(2, brojRedova), 1.5m, "zone-vip");

            if (brojRedova >= 12)
            {
                AddZone("Standard", 3, brojRedova - 5, 1.0m, "zone-standard");
                AddZone("Balkon", brojRedova - 4, brojRedova, 0.8m, "zone-balkon");
            }
            else
            {
                AddZone("Standard", 3, brojRedova, 1.0m, "zone-standard");
            }

            return zones;
        }
    }
}
