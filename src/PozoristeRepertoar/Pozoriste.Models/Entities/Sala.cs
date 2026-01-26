using System.ComponentModel.DataAnnotations;

namespace Pozoriste.Models.Entities
{
    public class Sala
    {
        public int SalaId { get; set; }
        public string Naziv { get; set; } = string.Empty;

        [Range(1, 200, ErrorMessage = "Broj redova mora biti izmedju 1 i 200.")]
        public int BrojRedova { get; set; }

        [Range(1, 200, ErrorMessage = "Broj sedista po redu mora biti izmedju 1 i 200.")]
        public int SedistaPoRedu { get; set; }

        public int Kapacitet { get; set; }
    }
}
