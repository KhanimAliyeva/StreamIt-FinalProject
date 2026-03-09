using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Season:BaseEntity
    {
        public int TVShowId { get; set; }
        public int SeasonNumber { get; set; }

        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }

   

}
