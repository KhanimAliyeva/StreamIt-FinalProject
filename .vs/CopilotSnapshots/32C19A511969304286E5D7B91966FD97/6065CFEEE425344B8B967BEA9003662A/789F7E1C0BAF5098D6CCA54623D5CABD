using STREAMIT.Business.Abstractions;
using STREAMIT.Business.Dtos.ResultDtos;

namespace STREAMIT.Presentation.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;


        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                ResultDto errorResult = new()
                {
                    IsSucceed = false,
                    Message = "An unexpected error occurred. Please try again later.",
                    StatusCode = 500
                };

                if (ex is IBaseException baseException)
                {
                    errorResult.Message = ex.Message;
                    errorResult.StatusCode = baseException.StatusCode;
                }
                await context.Response.WriteAsJsonAsync(errorResult);
            }


        }


    }
}
