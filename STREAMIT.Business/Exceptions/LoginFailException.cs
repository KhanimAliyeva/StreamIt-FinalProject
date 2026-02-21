using STREAMIT.Business.Abstractions;

namespace STREAMIT.Business.Exceptions
{
    public class LoginFailException(string message = "Login Failed...") : Exception(message), IBaseException
    {
        public int StatusCode { get; set; } = 400;
    }
}
