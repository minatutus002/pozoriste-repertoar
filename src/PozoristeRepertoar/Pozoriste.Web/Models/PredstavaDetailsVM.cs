using System;
using System.Collections.Generic;

namespace Pozoriste.Web.Models
{
    public class PredstavaDetailsVM
    {
        public int PredstavaId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Zanr { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public string? SlikaUrl { get; set; }
        public decimal Cena { get; set; }
        public int TrajanjeMin { get; set; }

        public List<TerminInfo> Termini { get; set; } = new();

        public class TerminInfo
        {
            public int TerminId { get; set; }
            public DateTime DatumVreme { get; set; }
            public string Sala { get; set; } = string.Empty;
            public int Kapacitet { get; set; }
            public int Slobodno { get; set; }
        }

        public List<string> Glumci { get; set; } = new();

    }
}
