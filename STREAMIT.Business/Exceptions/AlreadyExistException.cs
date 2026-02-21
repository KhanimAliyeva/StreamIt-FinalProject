using STREAMIT.Business.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Exceptions
{
    public class AlreadyExistException(string message = "This item is already exist...") : Exception(message), IBaseException
    {
        public int StatusCode { get; set; } = 400;
    }
}
