using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface IPersonService
    {
        Task<ResultDto> CreateAsync(CreatePersonDto dto);
        Task<ResultDto> UpdateAsync(UpdatePersonDto dto);
        Task<ResultDto> DeleteAsync(int id);
        Task<List<GetPersonDto>> GetAllAsync();
        Task<GetPersonDto> GetByIdAsync(int id);

    }
}
