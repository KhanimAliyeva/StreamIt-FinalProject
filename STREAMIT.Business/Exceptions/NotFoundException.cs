using STREAMIT.Business.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Exceptions
{
    public class NotFoundException(string message = "Object is not found...") : Exception(message), IBaseException
    {
        public int StatusCode { get; set; } = 404;

    }
}
