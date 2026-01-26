using System.Collections.Generic;
using Pozoriste.Models.Entities;

namespace Pozoriste.Web.Models
{
    public class HomeIndexVM
    {
        public List<Predstava> Predstave { get; set; } = new();
        public List<Sala> Sale { get; set; } = new();
        public List<Termin> Termini { get; set; } = new();
    }
}
