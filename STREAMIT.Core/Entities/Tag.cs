using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Tag:BaseEntity
    {
        public string Name { get; set; }=string.Empty;

        public ICollection<MovieTag> MovieTags { get; set; } = [];
        public ICollection<TvShowTag> TVShowTags { get; set; }= [];
    }



}
