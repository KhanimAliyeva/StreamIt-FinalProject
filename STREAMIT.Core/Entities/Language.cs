using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Language:BaseEntity
    {
        public string Name { get; set; }=string.Empty;
        public string Code { get; set; }=string.Empty;

        public ICollection<Movie> Movies { get; set; } = [];
    }

    
}
