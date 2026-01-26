using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pozoriste.Web.Models
{
    public class RepertoarIndexVM
    {
        public int? ZanrId { get; set; }
        public int? SalaId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public List<SelectListItem> Zanrovi { get; set; } = new();
        public List<SelectListItem> Sale { get; set; } = new();
        public List<RepertoarRowVM> Items { get; set; } = new();

        public bool HasFilters =>
            ZanrId.HasValue || SalaId.HasValue || DateFrom.HasValue || DateTo.HasValue;
    }
}
