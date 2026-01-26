using System;
using System.ComponentModel.DataAnnotations;

namespace Pozoriste.Models.Entities
{
    public class Termin
    {
        public int TerminId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Izaberite predstavu")]
        public int PredstavaId { get; set; }

        public Predstava? Predstava { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Izaberite salu")]
        public int SalaId { get; set; }

        public Sala? Sala { get; set; }

        public DateTime DatumVreme { get; set; }
    }
}
