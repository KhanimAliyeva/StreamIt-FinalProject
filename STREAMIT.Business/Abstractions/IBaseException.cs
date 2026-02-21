using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Abstractions
{
    public interface IBaseException
    {
        public int StatusCode { get; set; } 
    }
}
