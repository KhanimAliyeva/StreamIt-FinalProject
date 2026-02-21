using AutoMapper;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.PersonDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Exceptions;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Repositories.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STREAMIT.Business.Services.Implementations
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public PersonService(
            IPersonRepository repository,
            IMapper mapper,
            ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        // CREATE
        public async Task<ResultDto> CreateAsync(CreatePersonDto dto)
        {
            var person = _mapper.Map<Person>(dto);

            var imageUrl = await _cloudinaryService.FileCreateAsync(dto.Image);
            person.ImageUrl = imageUrl;

            await _repository.AddAsync(person);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Person created successfully.",
                StatusCode = 201
            };
        }

        // UPDATE
        public async Task<ResultDto> UpdateAsync(UpdatePersonDto dto)
        {
            var person = await _repository.GetByIdAsync(dto.Id);
            if (person == null)
                throw new NotFoundException("Person not found.");

            if (dto.Image != null)
            {
                var imageUrl = await _cloudinaryService.FileCreateAsync(dto.Image);

                if (!string.IsNullOrEmpty(person.ImageUrl))
                    await _cloudinaryService.FileDeleteAsync(person.ImageUrl);

                person.ImageUrl = imageUrl;
            }

            _mapper.Map(dto, person);
            _repository.Update(person);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Person updated successfully.",
                StatusCode = 200
            };
        }

        // DELETE
        public async Task<ResultDto> DeleteAsync(int id)
        {
            var person = await _repository.GetByIdAsync(id);
            if (person == null)
                throw new NotFoundException("Person not found.");

            _repository.Delete(person);
            await _repository.SaveChangesAsync();

            if (!string.IsNullOrEmpty(person.ImageUrl))
                await _cloudinaryService.FileDeleteAsync(person.ImageUrl);

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Person deleted successfully.",
                StatusCode = 200
            };
        }

        // GET ALL
        public async Task<List<GetPersonDto>> GetAllAsync()
        {
            var persons = await _repository.GetAll(true)
                .Include(x => x.MoviePeople).ThenInclude(x => x.Movie)
                .Include(x => x.TVShowPeople).ThenInclude(x => x.TvShow)
                .ToListAsync();

            return _mapper.Map<List<GetPersonDto>>(persons);
        }

        // GET BY ID
        public async Task<GetPersonDto> GetByIdAsync(int id)
        {
            var person = await _repository.GetByIdAsync(id);

            if (person == null)
                throw new NotFoundException("Person not found.");

            return _mapper.Map<GetPersonDto>(person);
        }
    }
}
