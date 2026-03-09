using STREAMIT.Core.Entities;
using STREAMIT.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.PaymentDtos
{
    public class MembershipGetVM
    {
        public int Id { get; set; }
        public string HppUrl { get; set; } = string.Empty;
        public string Password { get; set; }= string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Cvv2AuthStatus { get; set; }=string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
