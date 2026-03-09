using AutoMapper;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Exceptions;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace STREAMIT.Business.Services.Implementations
{
    public class GenreService(IGenreRepository _repository, IMapper _mapper) : IGenreService
    {
        public async Task CreateAsync(CreateGenreDto dto)
        {
            var isExistGenre= await _repository.AnyAsync(g => g.Name.ToLower() == dto.Name.ToLower());
            if(isExistGenre)
            {
                throw new AlreadyExistException("Genre is already exist.");
            }
            var genre = _mapper.Map<Genre>(dto);
            await _repository.AddAsync(genre);
            await _repository.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var genre = await _repository.GetAsync(x=>x.Id==id);
            if (genre is null)
            {
                throw new NotFoundException();
            }
            _repository.Delete(genre);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<GetGenreDto>> GetAllAsync()
        {
            var genres = await _repository.GetAll(true)
       .Include(g => g.MovieGenres)
           .ThenInclude(mg => mg.Movie)
      
       .ToListAsync();

            var dtos = genres.Select(g => new GetGenreDto
            {
                Id = g.Id,
                Name = g.Name,
                MovieCount = g.MovieGenres.Count(mg => mg.MovieId != 0),   // Id yox MovieId sayılır
            }).ToList();
            return dtos;
        }

        public async Task<GetGenreDto> GetByIdAsync(int id)
        {
            var genre = await _repository.GetAsync(x=>x.Id==id);
            if (genre is null)
            {
                throw new NotFoundException();
            }
            var dto = _mapper.Map<GetGenreDto>(genre);
            return dto;
        }

        public async Task UpdateAsync(UpdateGenreDto dto)
        {

            var genre = await _repository.GetByIdAsync(dto.Id);
            if (genre is null)
            {
                throw new NotFoundException("Genre is not found.");
            }

            var isExistGenre = await _repository.AnyAsync(g => g.Name.ToLower() == dto.Name.ToLower() && g.Id!=dto.Id);
            if (isExistGenre)
            {
                throw new AlreadyExistException("Genre is already exist.");
            }
            genre = _mapper.Map(dto, genre);
            _repository.Update(genre);
            await _repository.SaveChangesAsync();
        }
    }
}
