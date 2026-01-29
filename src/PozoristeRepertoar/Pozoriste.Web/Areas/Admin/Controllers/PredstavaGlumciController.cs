using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pozoriste.DataAccess.Repositories;
using Pozoriste.Web.Models;

namespace Pozoriste.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PredstavaGlumciController : Controller
    {
        private readonly IPredstavaRepository _predstavaRepo;
        private readonly IGlumacRepository _glumacRepo;

        public PredstavaGlumciController(IPredstavaRepository predstavaRepo, IGlumacRepository glumacRepo)
        {
            _predstavaRepo = predstavaRepo;
            _glumacRepo = glumacRepo;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var glumci = await _glumacRepo.GetAllAsync();
            var vm = glumci
                .Select(g => new GlumacSelectVM
                {
                    GlumacId = g.GlumacId,
                    PunoIme = g.PunoIme
                })
                .ToList();

            return Ok(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Selected(int predstavaId)
        {
            var ids = await _predstavaRepo.GetGlumacIdsAsync(predstavaId);
            return Ok(ids);
        }

        [HttpGet]
        public async Task<IActionResult> EditData(int predstavaId)
        {
            var predstava = await _predstavaRepo.GetByIdAsync(predstavaId);
            if (predstava == null) return NotFound();

            var glumci = await _glumacRepo.GetAllAsync();
            var selectedIds = await _predstavaRepo.GetGlumacIdsAsync(predstavaId);

            var vm = new PredstavaGlumciEditVM
            {
                PredstavaId = predstavaId,
                AllGlumci = glumci
                    .Select(g => new GlumacSelectVM
                    {
                        GlumacId = g.GlumacId,
                        PunoIme = g.PunoIme
                    })
                    .ToList(),
                SelectedGlumacIds = selectedIds
            };

            return Ok(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PredstavaGlumciEditVM vm)
        {
            if (vm == null) return BadRequest();

            var predstava = await _predstavaRepo.GetByIdAsync(vm.PredstavaId);
            if (predstava == null) return NotFound();

            var ids = vm.SelectedGlumacIds ?? new List<int>();
            await _predstavaRepo.UpdateGlumciAsync(vm.PredstavaId, ids);

            return Ok(new { success = true });
        }
    }
}
