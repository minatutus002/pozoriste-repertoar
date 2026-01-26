using Pozoriste.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pozoriste.DataAccess.Repositories;

namespace Pozoriste.Web.Controllers
{
    [Authorize]
    public class ProfilController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRezervacijaRepository _rezRepo;

        public ProfilController(UserManager<IdentityUser> userManager, IRezervacijaRepository rezRepo)
        {
            _userManager = userManager;
            _rezRepo = rezRepo;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var rezervacije = await _rezRepo.GetByUserAsync(user.Id);

            ViewBag.Email = user.Email;
            return View(rezervacije);
        }
    }
}
