using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class MovieTag:BaseEntity
    {
        public int MovieId { get; set; }
        public int TagId { get; set; }

        public Movie? Movie { get; set; }
        public Tag? Tag { get; set; }
    }
}
