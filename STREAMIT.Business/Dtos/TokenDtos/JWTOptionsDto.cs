using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.TokenDtos
{
    public class JWTOptionsDto
    {
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiredDate { get; set; }
    }
}
