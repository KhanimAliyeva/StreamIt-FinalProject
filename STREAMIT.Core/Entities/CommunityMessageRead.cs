using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class CommunityMessageRead
    {
        public int Id { get; set; }

        public long MessageId { get; set; }
        public CommunityMessage Message { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;

        public DateTime ReadAt { get; set; } = DateTime.UtcNow.AddHours(4);
    }
}
