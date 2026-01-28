using System.Collections.Generic;

namespace Pozoriste.Web.Models
{
    public class SalaIndexVm
    {
        public int SalaId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public int BrojRedova { get; set; }
        public int SedistaPoRedu { get; set; }
        public int Kapacitet { get; set; }

        public List<SalaZonaVm> Zone { get; set; } = new();
    }

    public class SalaZonaVm
    {
        public string Naziv { get; set; } = string.Empty;
        public string Redovi { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public string CssClass { get; set; } = string.Empty;
    }
}
