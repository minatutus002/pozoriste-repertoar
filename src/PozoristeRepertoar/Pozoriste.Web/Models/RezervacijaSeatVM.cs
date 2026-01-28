using System.Collections.Generic;

namespace Pozoriste.Web.Models
{
    public class RezervacijaSeatVM
    {
        public int TerminId { get; set; }
        public string Predstava { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public DateTime DatumVreme { get; set; }
        public decimal Cena { get; set; }

        public int BrojRedova { get; set; }
        public int SedistaPoRedu { get; set; }

        public List<SeatZoneVm> Zone { get; set; } = new();

        public HashSet<string> Zauzeta { get; set; } = new();
        public List<string> Selected { get; set; } = new();
    }
}
