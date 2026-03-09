using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Repositories.Abstractions.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface IMovieService
    {
        Task<ResultDto> CreateAsync(CreateMovieDto dto);
        Task<ResultDto> UpdateAsync(UpdateMovieDto dto);
        Task<ResultDto> DeleteAsync(int id);
        Task<List<GetMovieDto>> GetAllAsync();
        Task<GetMovieDto> GetByIdAsync(int id);
        Task<List<Movie>> GetAllWithDetailsAsync();
        Task<List<GetMovieDto>> GetUpcomingMoviesAsync();
        Task<List<GetMovieDto>> GetLatestMoviesAsync();
        Task<List<GetMovieDto>> GetBestInTvAsync();


    }
}
