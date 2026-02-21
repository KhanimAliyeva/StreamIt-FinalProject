using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TokenDtos;
using STREAMIT.Business.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface IAuthService
    {
        Task<ResultDto> RegisterAsync(RegisterDto dto);
        Task<ResultDto<AccessTokenDto>> LoginAsync(LoginDto dto);
        Task<ResultDto<AccessTokenDto>> RefreshTokenAsync(string token);
        Task<ResultDto> ConfirmEmailAsync(string userId, string token);
    }
}
