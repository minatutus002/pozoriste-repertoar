using System.ComponentModel.DataAnnotations;

namespace Pozoriste.Web.Models
{
    public class RezervacijaCreateVM
    {
        public int TerminId { get; set; }

        public string Predstava { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;

        public DateTime DatumVreme { get; set; }
        public decimal Cena { get; set; }

        public int Kapacitet { get; set; }
        public int Zauzeto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Broj karata mora biti veci od 0.")]
        public int BrojKarata { get; set; } = 1;

        public int Slobodno => Math.Max(0, Kapacitet - Zauzeto);
    }
}
