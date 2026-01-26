using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pozoriste.DataAccess.Repositories;
using Pozoriste.Web.Models;

namespace Pozoriste.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IPredstavaRepository _predstavaRepo;
        private readonly ISalaRepository _salaRepo;
        private readonly ITerminRepository _terminRepo;

        public HomeController(
            IPredstavaRepository predstavaRepo,
            ISalaRepository salaRepo,
            ITerminRepository terminRepo)
        {
            _predstavaRepo = predstavaRepo;
            _salaRepo = salaRepo;
            _terminRepo = terminRepo;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeIndexVM
            {
                Predstave = (await _predstavaRepo.GetAllAsync()).ToList(),
                Sale = (await _salaRepo.GetAllAsync()).ToList(),
                Termini = (await _terminRepo.GetAllWithDetailsAsync()).ToList()
            };

            return View(vm);
        }
    }
}
