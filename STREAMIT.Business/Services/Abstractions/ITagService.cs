using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Business.Dtos.TagDtos.STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface ITagService
    {
        Task<ResultDto> CreateAsync(CreateTagDto dto);
        Task<ResultDto> UpdateAsync(UpdateTagDto dto);
        Task<ResultDto> DeleteAsync(int id);
        Task<List<GetTagDto>> GetAllAsync();
        Task<GetTagDto> GetByIdAsync(int id);
    }
}
