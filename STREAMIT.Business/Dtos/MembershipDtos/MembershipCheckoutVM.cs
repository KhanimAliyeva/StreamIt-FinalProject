using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.MembershipDtos
{
    public class MembershipCheckoutVM
    {
        public int Id { get; set; }
        public Membership Membership { get; set; } = null!;

        public string Name { get; set; } = string.Empty; 
        public decimal Price { get; set; }          

        public string Currency { get; set; } = "AZN"; 
        public string RedirectUrl { get; set; } = string.Empty; 

        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
