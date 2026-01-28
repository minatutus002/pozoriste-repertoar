using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pozoriste.DataAccess.Repositories;
using Pozoriste.Web.Models;

using Microsoft.AspNetCore.Authorization;

namespace Pozoriste.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTerminController : Controller
    {
        private readonly ITerminRepository _terminRepo;
        private readonly IPredstavaRepository _predstavaRepo;
        private readonly ISalaRepository _salaRepo;

        public AdminTerminController(
            ITerminRepository terminRepo,
            IPredstavaRepository predstavaRepo,
            ISalaRepository salaRepo)
        {
            _terminRepo = terminRepo;
            _predstavaRepo = predstavaRepo;
            _salaRepo = salaRepo;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var termini = await _terminRepo.GetAllWithDetailsAsync();
            return View(termini);
        }

        // =========================
        // CREATE GET
        // =========================
        public async Task<IActionResult> Create()
        {
            var vm = new TerminCreateVM();

            await NapuniListe(vm);

            return View(vm);
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TerminCreateVM vm)
        {
            var predstava = await _predstavaRepo.GetByIdAsync(vm.PredstavaId);
            if (predstava == null)
                ModelState.AddModelError(nameof(vm.PredstavaId), "Izabrana predstava ne postoji.");
            else if (predstava.TrajanjeMin <= 0)
                ModelState.AddModelError(nameof(vm.PredstavaId), "Trajanje predstave nije ispravno.");

            if (vm.DatumVreme == null)
                ModelState.AddModelError(nameof(vm.DatumVreme), "Morate izabrati datum i vreme.");

            if (!ModelState.IsValid)
            {
                await NapuniListe(vm);
                return View(vm);
            }

            if (vm.DatumVreme == null)
            {
                await NapuniListe(vm);
                return View(vm);
            }

            var datum = vm.DatumVreme.Value;
            var end = datum.AddMinutes(predstava!.TrajanjeMin);

            var overlap = await _terminRepo.GetOverlapAsync(vm.SalaId, datum, end);
            if (overlap != null)
            {
                var naziv = overlap.Predstava?.Naziv ?? "nepoznata predstava";
                var vreme = overlap.DatumVreme.ToString("dd.MM.yyyy HH:mm");
                ModelState.AddModelError(nameof(vm.DatumVreme), $"Termin se preklapa sa \"{naziv}\" ({vreme}).");
                await NapuniListe(vm);
                return View(vm);
            }

            await _terminRepo.CreateAsync(vm.PredstavaId, vm.SalaId, datum);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT GET
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var termin = await _terminRepo.GetByIdAsync(id);

            if (termin == null)
                return NotFound();

            var vm = new TerminCreateVM
            {
                PredstavaId = termin.PredstavaId,
                SalaId = termin.SalaId,
                DatumVreme = termin.DatumVreme
            };

            await NapuniListe(vm);

            ViewBag.TerminId = id;

            return View(vm);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TerminCreateVM vm)
        {
            var predstava = await _predstavaRepo.GetByIdAsync(vm.PredstavaId);
            if (predstava == null)
                ModelState.AddModelError(nameof(vm.PredstavaId), "Izabrana predstava ne postoji.");
            else if (predstava.TrajanjeMin <= 0)
                ModelState.AddModelError(nameof(vm.PredstavaId), "Trajanje predstave nije ispravno.");

            if (vm.DatumVreme == null)
                ModelState.AddModelError(nameof(vm.DatumVreme), "Morate izabrati datum i vreme.");

            if (!ModelState.IsValid)
            {
                await NapuniListe(vm);
                ViewBag.TerminId = id;
                return View(vm);
            }

            if (vm.DatumVreme == null)
            {
                await NapuniListe(vm);
                ViewBag.TerminId = id;
                return View(vm);
            }

            var datum = vm.DatumVreme.Value;
            var end = datum.AddMinutes(predstava!.TrajanjeMin);

            var overlap = await _terminRepo.GetOverlapAsync(vm.SalaId, datum, end, id);
            if (overlap != null)
            {
                var naziv = overlap.Predstava?.Naziv ?? "nepoznata predstava";
                var vreme = overlap.DatumVreme.ToString("dd.MM.yyyy HH:mm");
                ModelState.AddModelError(nameof(vm.DatumVreme), $"Termin se preklapa sa \"{naziv}\" ({vreme}).");
                await NapuniListe(vm);
                ViewBag.TerminId = id;
                return View(vm);
            }

            await _terminRepo.UpdateAsync(id, vm.PredstavaId, vm.SalaId, datum);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            await _terminRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // POMOĆNA METODA
        // =========================
        private async Task NapuniListe(TerminCreateVM vm)
        {
            vm.Predstave = (await _predstavaRepo.GetAllAsync())
                .Select(p => new SelectListItem
                {
                    Value = p.PredstavaId.ToString(),
                    Text = p.Naziv
                })
                .ToList();

            vm.Sale = (await _salaRepo.GetAllAsync())
                .Select(s => new SelectListItem
                {
                    Value = s.SalaId.ToString(),
                    Text = s.Naziv
                })
                .ToList();
        }

    }

}
