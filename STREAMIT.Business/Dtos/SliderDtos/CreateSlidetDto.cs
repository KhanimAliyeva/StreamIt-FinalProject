using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.SliderDtos
{
    public class CreateSliderDto
    {
        public int MovieId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }

}
