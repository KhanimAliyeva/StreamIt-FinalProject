using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.UserDtos
{
   
        public class UpdateMeDto
        {
            public string FullName { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Email { get; set; } = "";
        }

        public class MeDto
        {
            public string FullName { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Email { get; set; } = "";
        }
    }
