using Pozoriste.Models.Entities;

namespace Pozoriste.Web.Models
{
    public class RezervacijaCheckoutVM
    {
        public int RezervacijaId { get; set; }
        public string Predstava { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime DatumVreme { get; set; }
        public int BrojKarata { get; set; }
        public decimal Cena { get; set; }
        public string Sedista { get; set; } = string.Empty;
        public RezervacijaStatus Status { get; set; }

        public decimal CenaPoKarti { get; set; }
        public decimal Ukupno { get; set; }
        public decimal ProsecnaCenaKarte { get; set; }
        public List<ZonaCenaStavkaVM> StavkePoZonama { get; set; } = new();

        public List<PriceLineVm> Stavke { get; set; } = new();




    }

    public class ZonaCenaStavkaVM
    {
        public string Zona { get; set; } = "";
        public int Kolicina { get; set; }
        public decimal CenaJedne { get; set; }
        public decimal Ukupno => decimal.Round(CenaJedne * Kolicina, 2);
    }

    public class PriceLineVm
    {
        public string Zona { get; set; } = "";
        public int Kolicina { get; set; }
        public decimal CenaJed { get; set; }
        public decimal Ukupno { get; set; }
    }

}
