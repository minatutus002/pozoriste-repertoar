using Microsoft.AspNetCore.Mvc;
using Pozoriste.DataAccess.Repositories;

namespace Pozoriste.Web.Controllers
{
    public class TerminController : Controller
    {
        private readonly ITerminRepository _repo;

        public TerminController(ITerminRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var termini = await _repo.GetAllAsync();
            return View(termini);
        }
    }
}
