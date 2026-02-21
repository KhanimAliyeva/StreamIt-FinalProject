using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Episode : BaseEntity
    {
        public int SeasonId { get; set; }
        public string Title { get; set; }=string.Empty;
        public int EpisodeNumber { get; set; }
        public float Duration { get; set; }
        public string VideoUrl { get; set; }= string.Empty;

        public Season? Season { get; set; }
    }
}
