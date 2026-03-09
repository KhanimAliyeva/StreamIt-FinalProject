using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public string UserId { get; set; }= string.Empty;
        public int MembershipId { get; set; }

        public decimal Amount { get; set; }

        public string OrderId { get; set; } = string.Empty;  
        public string Status { get; set; }  =string.Empty; 
        public string RedirectUrl { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
