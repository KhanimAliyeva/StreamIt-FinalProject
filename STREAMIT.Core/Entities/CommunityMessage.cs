using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class CommunityMessage
    {
        public long Id { get; set; }

        public int GroupId { get; set; }
        public CommunityGroup Group { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;

        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow.AddHours(4);

        public ICollection<CommunityMessageRead> Reads { get; set; } = new List<CommunityMessageRead>();
    }
}
