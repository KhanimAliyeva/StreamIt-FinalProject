using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Membership : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public int DurationInDays { get; set; }

        public string VideoQuality { get; set; } = string.Empty;
        public int MaxDevices { get; set; }

        public bool HasAds { get; set; }
        public bool CanDownload { get; set; }
        public bool IsActive { get; set; }

        public int PriorityLevel { get; set; }

        public ICollection<Movie> Movies { get; set; } = [];
        public ICollection<TVShow> TVShows { get; set; } = [];
        public ICollection<UserMembership> UserMemberships { get; set; } = [];
    }

}
