using Microsoft.AspNetCore.Mvc;
using Pozoriste.DataAccess.Repositories;

namespace Pozoriste.Web.Controllers
{
    public class SalaController : Controller
    {
        private readonly ISalaRepository _salaRepo;

        public SalaController(ISalaRepository salaRepo)
        {
            _salaRepo = salaRepo;
        }

        public async Task<IActionResult> Index()
        {
            var sale = await _salaRepo.GetAllAsync();
            return View(sale);
        }
    }
}
