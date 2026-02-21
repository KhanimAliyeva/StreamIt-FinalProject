using STREAMIT.Business.Dtos.GenreDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface IGenreService
    {

        Task CreateAsync(CreateGenreDto dto);
        Task UpdateAsync(UpdateGenreDto dto);
        Task DeleteAsync(int id);
        Task<List<GetGenreDto>> GetAllAsync();
        Task<GetGenreDto> GetByIdAsync(int id);
    }
}
