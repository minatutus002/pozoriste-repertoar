using Pozoriste.Models.Entities;

namespace Pozoriste.Web.Models
{
    public class AdminRezervacijaRowVM
    {
        public int RezervacijaId { get; set; }
        public string KorisnikEmail { get; set; } = string.Empty;
        public string Predstava { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime DatumVreme { get; set; }
        public int BrojKarata { get; set; }
        public decimal Cena { get; set; }
        public RezervacijaStatus Status { get; set; }
        public string Sedista { get; set; } = string.Empty;

        public decimal Ukupno => Cena * BrojKarata;
    }
}
