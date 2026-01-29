using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pozoriste.Web.Models
{
    public class PredstavaAdminVM
    {
        public int PredstavaId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public string? SlikaUrl { get; set; }
        public decimal Cena { get; set; }
        public int TrajanjeMin { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Izaberite zanr.")]
        public int ZanrId { get; set; }

        public List<SelectListItem> Glumci { get; set; } = new();
        public List<int> SelectedGlumciIds { get; set; } = new();
    }
}
