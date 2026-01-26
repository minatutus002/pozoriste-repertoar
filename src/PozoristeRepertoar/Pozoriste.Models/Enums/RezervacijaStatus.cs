namespace Pozoriste.Models.Entities
{
    public enum RezervacijaStatus
    {
        Rezervisano = 0,
        Placeno = 1,
        Otkazano = 2,
        OtkazivanjeNaCekanju = 3,
        OtkazivanjeNaCekanjuPlaceno = 4,
        Refundiran = 5
    }
}
