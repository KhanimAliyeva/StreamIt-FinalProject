using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Slider2 : BaseEntity
    {

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
