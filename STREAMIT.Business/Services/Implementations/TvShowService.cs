using AutoMapper;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TvShowDtos;
using STREAMIT.Business.Exceptions;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Implementations
{
    public class TvShowService(ITvShowRepository _repository, IMapper _mapper) : ITvShowService
    {
       


        public async Task<ResultDto> DeleteAsync(int id)
        {
            var tvShow = await _repository.GetByIdAsync(id);
            if (tvShow is null)
            {
                throw new NotFoundException("Show is not found.");
            }
            _repository.Delete(tvShow);
            await _repository.SaveChangesAsync();
            return new ResultDto(message: "Deleted Successfully");
        }
        public async Task<List<GetTvShowDto>> GetAllAsync()
        {
            var tvShows = await _repository.GetAll(true)
                .Include(x => x.TvShowGenres).ThenInclude(x => x.Genre)
                .Include(x => x.TvShowPeople).ThenInclude(x => x.Person)
                .Include(x => x.TvShowTags).ThenInclude(x => x.Tag)
                .Include(x => x.Seasons)
                .Include(x => x.Membership)
                .Include(x => x.Language)
                .ToListAsync();

            return _mapper.Map<List<GetTvShowDto>>(tvShows);
        }

        public async Task<GetTvShowDto> GetByIdAsync(int id)
        {
            var tvShow = await _repository.GetAll()
                .Include(x => x.TvShowGenres).ThenInclude(x => x.Genre)
                .Include(x => x.TvShowPeople).ThenInclude(x => x.Person)
                .Include(x => x.TvShowTags).ThenInclude(x => x.Tag)
                .Include(x => x.Seasons)
                .Include(x => x.Membership)
                .Include(x => x.Language)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tvShow is null)
                throw new NotFoundException("TV Show not found.");

            return _mapper.Map<GetTvShowDto>(tvShow);
        }


        public async Task<ResultDto> CreateAsync(CreateTvShowDto dto)
        {
            var tvShow = _mapper.Map<TVShow>(dto);

            // Poster upload varsa əlavə et
            if (!string.IsNullOrEmpty(dto.PosterUrl))
                tvShow.PosterUrl = dto.PosterUrl;

            // GenreIds
            if (dto.GenreIds != null && dto.GenreIds.Any())
            {
                tvShow.TvShowGenres = dto.GenreIds
                    .Select(id => new TvShowGenre { GenreId = id })
                    .ToList();
            }

            // PersonIds
            if (dto.PersonIds != null && dto.PersonIds.Any())
            {
                tvShow.TvShowPeople = dto.PersonIds
                    .Select(id => new TvShowPerson { PersonId = id })
                    .ToList();
            }

            // TagIds
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                tvShow.TvShowTags = dto.TagIds
                    .Select(id => new TvShowTag { TagId = id })
                    .ToList();
            }

            await _repository.AddAsync(tvShow);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "TV Show created successfully",
                StatusCode = 201
            };
        }

        public async Task<ResultDto> UpdateAsync(UpdateTvShowDto dto)
        {
            var tvShow = await _repository.GetByIdAsync(dto.Id);
            if (tvShow is null)
                throw new NotFoundException("TV Show not found.");

            // Poster update varsa
            if (!string.IsNullOrEmpty(dto.PosterUrl))
                tvShow.PosterUrl = dto.PosterUrl;

            _mapper.Map(dto, tvShow);

            // GenreIds
            if (dto.GenreIds != null)
            {
                tvShow.TvShowGenres.Clear();
                tvShow.TvShowGenres = dto.GenreIds
                    .Select(id => new TvShowGenre { TvShowId = dto.Id, GenreId = id })
                    .ToList();
            }

            // PersonIds
            if (dto.PersonIds != null)
            {
                tvShow.TvShowPeople.Clear();
                tvShow.TvShowPeople = dto.PersonIds
                    .Select(id => new TvShowPerson { TvShowId = dto.Id, PersonId = id })
                    .ToList();
            }

            // TagIds
            if (dto.TagIds != null)
            {
                tvShow.TvShowTags.Clear();
                tvShow.TvShowTags = dto.TagIds
                    .Select(id => new TvShowTag { TvShowId = dto.Id, TagId = id })
                    .ToList();
            }

            _repository.Update(tvShow);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "TV Show updated successfully",
                StatusCode = 200
            };
        }


    }
}
