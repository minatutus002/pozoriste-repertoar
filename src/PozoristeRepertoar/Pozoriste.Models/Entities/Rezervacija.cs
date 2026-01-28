using System.Collections.Generic;

namespace Pozoriste.Models.Entities
{
    public class Rezervacija
    {
        public int RezervacijaId { get; set; }

        public string KorisnikId { get; set; } = default!;

        public int TerminId { get; set; }
        public Termin Termin { get; set; } = default!;

        public int BrojKarata { get; set; }
        public DateTime DatumKreiranja { get; set; } = DateTime.Now;

        public RezervacijaStatus Status { get; set; } = RezervacijaStatus.Rezervisano;

        public ICollection<RezervacijaSediste> Sedista { get; set; } = new List<RezervacijaSediste>();

        public decimal UkupnaCena { get; set; }

    }
}
