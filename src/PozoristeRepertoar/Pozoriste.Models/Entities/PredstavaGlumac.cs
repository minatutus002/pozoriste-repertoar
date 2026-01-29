using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pozoriste.Models.Entities
{
    public class PredstavaGlumac
    {
        public int PredstavaId { get; set; }
        public Predstava Predstava { get; set; } = null!;

        public int GlumacId { get; set; }
        public Glumac Glumac { get; set; } = null!;
    }
}
