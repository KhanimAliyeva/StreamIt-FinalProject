using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class UserMembership : BaseEntity
    {
        public string  UserId { get; set; } = string.Empty;
        public int MembershipId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsTrial { get; set; }

        public decimal PaidAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public bool AutoRenew { get; set; }
        public DateTime? CancelledAt { get; set; }

        public AppUser? User { get; set; }
        public Membership? Membership { get; set; }
    }

}
