using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.ResultDtos
{
    public class ResultDto
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ResultDto()
        {
            IsSucceed = true;
            Message = "Successfully Completed";
            StatusCode = 200;
        }

        public ResultDto(string message):this()
        {
            Message = message;
        }

        public ResultDto(string message,int statusCode):this(message)
        {
            Message = message; 
            StatusCode = statusCode;
        }

        public ResultDto(string message, int statusCode, bool isSucceed):this(message,statusCode)
        {
            IsSucceed = isSucceed;
        }
    }

    public class ResultDto<T> : ResultDto
    {
        public T? Data { get; set; }
        public ResultDto() : base()
        {
        }
        public ResultDto(T data) : this()
        {
            Data = data;
        }
        public ResultDto(string message, T data) : base(message)
        {
            Data = data;
        }
        public ResultDto(string message, int statusCode, T data) : base(message, statusCode)
        {
            Data = data;
        }
        public ResultDto(string message, int statusCode, bool isSucced, T data) : base(message, statusCode, isSucced)
        {
            Data = data;
        }
    }
}
