using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class CommunityGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<CommunityMessage> Messages { get; set; } = new List<CommunityMessage>();
    }
}
