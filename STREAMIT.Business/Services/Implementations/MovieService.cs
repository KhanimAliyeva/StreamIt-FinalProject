using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.GenreDtos;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Exceptions;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using STREAMIT.DataAccess.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STREAMIT.Business.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _context;

        public MovieService(IMovieRepository repository, IMapper mapper, ICloudinaryService cloudinaryService, AppDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _context = context;
        }

        public async Task<ResultDto> CreateAsync(CreateMovieDto dto)
        {
            var movie = _mapper.Map<Movie>(dto);

            // Poster upload (only if file provided)
            if (dto.Poster != null)
            {
                var poster = await _cloudinaryService.FileCreateAsync(dto.Poster, "image");
                movie.PosterUrl = poster;
            }

            movie.YoutubeUrl = dto.YoutubeUrl;

            // Movie file upload (only if file provided)
            if (dto.Movie != null)
            {
                var movieFile = await _cloudinaryService.FileCreateAsync(dto.Movie, "video");
                movie.MovieUrl = movieFile;
            }

            if (dto.GenreIds != null && dto.GenreIds.Any())
            {
                movie.MovieGenres = dto.GenreIds
                    .Select(id => new MovieGenre { GenreId = id })
                    .ToList();
            }
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                movie.MovieTags = dto.TagIds
                    .Select(id => new MovieTag { TagId = id })
                    .ToList();
            }
            if (dto.PersonIds != null && dto.PersonIds.Any())
            {
                movie.MoviePeople = dto.PersonIds
                    .Select(id => new MoviePerson { PersonId = id })
                    .ToList();
            }


            await _repository.AddAsync(movie);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Movie is created successfully.",
                StatusCode = 201
            };
        }

        public async Task<ResultDto> DeleteAsync(int id)
        {
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null)
                throw new NotFoundException("Movie not found.");

            _repository.Delete(movie);
            await _repository.SaveChangesAsync();

            if (!string.IsNullOrEmpty(movie.PosterUrl))
                await _cloudinaryService.FileDeleteAsync(movie.PosterUrl);
            if (!string.IsNullOrEmpty(movie.YoutubeUrl))
                await _cloudinaryService.FileDeleteAsync(movie.YoutubeUrl);
            if (!string.IsNullOrEmpty(movie.MovieUrl))
                await _cloudinaryService.FileDeleteAsync(movie.MovieUrl);


            return new ResultDto
            {
                IsSucceed = true,
                Message = "Movie deleted successfully.",
                StatusCode = 200
            };
        }

        public async Task<List<GetMovieDto>> GetAllAsync()
        {
            var movies = await _repository.GetAll(true)

                .Include(x => x.Language)
                .Include(x => x.Membership)
                .Include(x => x.MovieStatistics)
                .Include(x => x.MovieGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieTags).ThenInclude(x => x.Tag)
                .Include(x => x.MoviePeople).ThenInclude(x => x.Person)
                .ToListAsync();


            var dtos = _mapper.Map<List<GetMovieDto>>(movies);
            return dtos;
        }

        public async Task<GetMovieDto> GetByIdAsync(int id)
        {
            // Load movie with related data so mapping has Genres/Tags/People populated
            var movie = await _repository.GetAll(true)
                .Include(x => x.Language)
                .Include(x => x.Membership)
                .Include(x => x.MovieStatistics)
                .Include(x => x.MovieGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieTags).ThenInclude(x => x.Tag)
                .Include(x => x.MoviePeople).ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                throw new NotFoundException("Movie not found.");

            var dto = _mapper.Map<GetMovieDto>(movie);
            return dto;
        }

        public async Task<ResultDto> UpdateAsync(UpdateMovieDto dto)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MoviePeople)
                .FirstOrDefaultAsync(m => m.Id == dto.Id); if (movie == null)
                throw new NotFoundException("Movie not found.");

            // Map scalar properties from DTO to entity first (this will not handle file uploads)
            _mapper.Map(dto, movie);

            // Poster upload (only if file provided)
            if (dto.Poster != null)
            {
                var posterUrl = await _cloudinaryService.FileCreateAsync(dto.Poster, "image");
                if (!string.IsNullOrEmpty(movie.PosterUrl))
                    await _cloudinaryService.FileDeleteAsync(movie.PosterUrl);
                movie.PosterUrl = posterUrl;
            }

            // Movie file upload (only if file provided)
            if (dto.Movie != null)
            {
                var movieUrl = await _cloudinaryService.FileCreateAsync(dto.Movie,"video");
                if (!string.IsNullOrEmpty(movie.MovieUrl))
                    await _cloudinaryService.FileDeleteAsync(movie.MovieUrl);
                movie.MovieUrl = movieUrl;
            }

            // Update many-to-many / join collections explicitly and ensure FK values are set
            if (dto.GenreIds != null)
            {
                movie.MovieGenres = dto.GenreIds
                    .Select(id => new MovieGenre { MovieId = movie.Id, GenreId = id })
                    .ToList();
            }

            if (dto.PersonIds?.Any() == true)
            {
                movie.MoviePeople = dto.PersonIds
                    .Select(id => new MoviePerson
                    {
                        PersonId = id,
                        MovieId = movie.Id
                    })
                    .ToList();
            }
            _repository.Update(movie);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Movie updated successfully.",
                StatusCode = 200
            };
        }

        public async Task<List<Movie>> GetAllWithDetailsAsync()
        {
            return await _repository.GetAll()

                .Include(x => x.Language)
                .Include(x => x.Membership)
                .Include(x => x.MovieStatistics)
                .Include(x => x.MovieGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieTags).ThenInclude(x => x.Tag)
                .Include(x => x.MoviePeople).ThenInclude(x => x.Person)
                .ToListAsync();
        }

        public async Task<List<GetMovieDto>> GetUpcomingMoviesAsync()
        {
            return await _repository.GetAll(true)
                .Where(m => m.ReleaseDate > DateTime.UtcNow)
                .Select(m => new GetMovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    PosterUrl = m.PosterUrl,
                    LanguageName = m.Language.Name,
                    Genres = m.MovieGenres
                        .Select(g => new GenreDto
                        {
                            Id = g.Genre.Id,
                            Name = g.Genre.Name
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null)
                throw new NotFoundException("Movie not found.");
            if (movie.MovieStatistics == null)
            {
                movie.MovieStatistics = new MovieStatistics
                {
                    MovieId = id,
                    ViewCount = 1,
                };
            }
            else
            {
                movie.MovieStatistics.ViewCount += 1;
            }
            _repository.Update(movie);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<GetMovieDto>> GetBestInTvAsync()
        {
            var movies = await _repository.GetAll(true)
                .Where(x => x.AverageRating > 0)
                .OrderByDescending(x => x.AverageRating)
                .Take(10)
                .Select(x => new GetMovieDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    PosterUrl = x.PosterUrl,
                    AverageRating = x.AverageRating,
                    LanguageName = x.Language.Name,
                    MembershipName = x.Membership.Name,
                    Genres = x.MovieGenres
                        .Select(g => new GenreDto
                        {
                            Id = g.Genre.Id,
                            Name = g.Genre.Name
                        }).ToList()
                })
                .ToListAsync();

            return movies;
        }

        public async Task<List<GetMovieDto>> GetLatestMoviesAsync()
        {
            var movies = await _repository.GetAll(true)
                .Where(x => x.CreatedDate <= DateTimeOffset.UtcNow)
                .OrderByDescending(x => x.CreatedDate)
                .Take(10)
                .Include(x => x.MovieGenres).ThenInclude(x => x.Genre)
                .AsSplitQuery()
                .ToListAsync();

            return _mapper.Map<List<GetMovieDto>>(movies);
        }

   
    }
}