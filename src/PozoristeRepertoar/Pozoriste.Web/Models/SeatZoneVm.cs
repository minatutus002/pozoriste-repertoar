namespace Pozoriste.Web.Models
{
    public class SeatZoneVm
    {
        public string Naziv { get; set; } = string.Empty;
        public int OdReda { get; set; }
        public int DoReda { get; set; }
        public decimal CenaMultiplier { get; set; }
        public string CssClass { get; set; } = string.Empty;
    }
}
