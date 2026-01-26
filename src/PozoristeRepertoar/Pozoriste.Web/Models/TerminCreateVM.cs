using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pozoriste.Web.Models
{
    public class TerminCreateVM
    {
        [Range(1, int.MaxValue, ErrorMessage = "Izaberite predstavu")]
        public int PredstavaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Izaberite salu")]
        public int SalaId { get; set; }

        [Required(ErrorMessage = "Morate izabrati datum i vreme")]
        public DateTime? DatumVreme { get; set; }

        public List<SelectListItem> Predstave { get; set; } = new();
        public List<SelectListItem> Sale { get; set; } = new();
    }
}
