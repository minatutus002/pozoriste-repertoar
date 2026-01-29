namespace Pozoriste.Web.Models
{
    public class PredstavaGlumciEditVM
    {
        public int PredstavaId { get; set; }
        public List<int> SelectedGlumacIds { get; set; } = new();
        public List<GlumacSelectVM> AllGlumci { get; set; } = new();
    }
}
