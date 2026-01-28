using Microsoft.AspNetCore.Mvc;
using Pozoriste.DataAccess.Repositories;
using Pozoriste.Web.Models;
using System.Collections.Generic;
using System.Linq;

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

            var model = sale.Select(s => new SalaIndexVm
            {
                SalaId = s.SalaId,
                Naziv = s.Naziv,
                BrojRedova = s.BrojRedova,
                SedistaPoRedu = s.SedistaPoRedu,
                Kapacitet = s.Kapacitet,
                Zone = BuildZoneForSala(s.SalaId, s.BrojRedova)
            }).ToList();

            return View(model);
        }

        private static List<SalaZonaVm> BuildZoneForSala(int salaId, int brojRedova)
        {
            var zones = new List<SalaZonaVm>();

            void AddZone(string naziv, int from, int to, string opis, string css)
            {
                if (from > to || from < 1 || to < 1) return;
                zones.Add(new SalaZonaVm
                {
                    Naziv = naziv,
                    Redovi = $"{RowLabel(from)}-{RowLabel(to)}",
                    Opis = opis,
                    CssClass = css
                });
            }

            if (brojRedova <= 0)
                return zones;

            if (salaId == 1)
            {
                AddZone("VIP zona", 1, 2, "Premium tretman i najbolji pogled.", "zone-vip");
                AddZone("Premium parter", 3, Math.Min(6, brojRedova),
                    "Najbolja akustika i komfor.", "zone-premium");
                if (brojRedova >= 10)
                {
                    AddZone("Balkon", Math.Max(brojRedova - 3, 7), brojRedova,
                        "Najpovoljnija mesta.", "zone-balkon");
                }
                return zones;
            }

            if (salaId == 2)
            {
                AddZone("VIP zona", 1, 2, "Premium tretman u prvim redovima.", "zone-vip");
                if (brojRedova >= 6)
                {
                    AddZone("Standard", 3, brojRedova - 2, "Udobna mesta u sredini sale.", "zone-standard");
                    AddZone("Intimna premijera", brojRedova - 1, brojRedova,
                        "Mala, tiha i intimna zona.", "zone-intimna");
                }
                else
                {
                    AddZone("Standard", 3, brojRedova, "Udobna mesta u sredini sale.", "zone-standard");
                }
                return zones;
            }

            AddZone("VIP zona", 1, Math.Min(2, brojRedova), "Premium tretman u prvim redovima.", "zone-vip");

            var balkonPostoji = brojRedova >= 12;
            if (balkonPostoji)
            {
                AddZone("Standard", 3, brojRedova - 5, "Najcesce birana zona.", "zone-standard");
                AddZone("Balkon", brojRedova - 4, brojRedova, "Povoljna zona sa visine.", "zone-balkon");
            }
            else
            {
                AddZone("Standard", 3, brojRedova, "Najcesce birana zona.", "zone-standard");
            }

            return zones;
        }

        private static string RowLabel(int row)
        {
            if (row >= 1 && row <= 26)
                return ((char)('A' + row - 1)).ToString();

            return row.ToString();
        }
    }
}
