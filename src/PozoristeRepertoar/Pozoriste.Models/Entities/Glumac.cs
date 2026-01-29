using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pozoriste.Models.Entities
{
    public class Glumac
    {
        public int GlumacId { get; set; }

        [Required, MaxLength(120)]
        public string PunoIme { get; set; } = string.Empty;

        // navigacija (many-to-many)
        public ICollection<PredstavaGlumac> Predstave { get; set; } = new List<PredstavaGlumac>();
    }
}
