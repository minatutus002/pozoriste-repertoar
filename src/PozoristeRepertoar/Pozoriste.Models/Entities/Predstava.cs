using System.ComponentModel.DataAnnotations;

namespace Pozoriste.Models.Entities
{
    public class Predstava
    {
        public int PredstavaId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public string? SlikaUrl { get; set; }
        public decimal Cena { get; set; }
        public int TrajanjeMin { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Izaberite zanr.")]
        public int ZanrId { get; set; }

        public Zanr? Zanr { get; set; }

        public ICollection<PredstavaGlumac> Glumci { get; set; } = new List<PredstavaGlumac>();

    }
}
