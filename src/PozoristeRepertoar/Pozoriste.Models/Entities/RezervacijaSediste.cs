namespace Pozoriste.Models.Entities
{
    public class RezervacijaSediste
    {
        public int RezervacijaSedisteId { get; set; }

        public int RezervacijaId { get; set; }
        public Rezervacija Rezervacija { get; set; } = default!;

        public int TerminId { get; set; }
        public Termin Termin { get; set; } = default!;

        public int Red { get; set; }
        public int Broj { get; set; }

        public decimal Cena { get; set; }
        public string Zona { get; set; } = "";

    }
}
