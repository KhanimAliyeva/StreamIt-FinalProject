using STREAMIT.Business.Abstractions;

namespace STREAMIT.Business.Exceptions
{
    public class RegisterFailException(string message = "Register Failed...") : Exception(message), IBaseException
    {
        public int StatusCode { get; set; } = 400;
    }
}
