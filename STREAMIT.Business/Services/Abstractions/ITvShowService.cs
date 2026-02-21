using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TvShowDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface ITvShowService
    {
        Task<ResultDto> CreateAsync(CreateTvShowDto dto);
        Task<ResultDto> UpdateAsync(UpdateTvShowDto dto);
        Task<ResultDto> DeleteAsync(int id);
        Task<List<GetTvShowDto>> GetAllAsync();
        Task<GetTvShowDto> GetByIdAsync(int id);
    }
}
