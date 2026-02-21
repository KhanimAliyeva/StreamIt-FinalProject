using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.UserDtos
{
    public class RegisterDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool AcceptTerms { get; set; }

    }
}
