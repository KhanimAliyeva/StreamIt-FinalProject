using AutoMapper;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Exceptions;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Repositories.Abstractions;
using STREAMIT.DataAccess.Repositories.Abstractions.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STREAMIT.Business.Services.Implementations
{
    public class LanguageService : ILanguageService
    {
        private readonly IRepository<Language> _repository;
        private readonly IMapper _mapper;

        public LanguageService(IRepository<Language> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultDto> CreateAsync(CreateLanguageDto dto)
        {
            var exists = await _repository.AnyAsync(x => x.Name == dto.Name);
            if (exists)
                return new ResultDto { IsSucceed = false, Message = "Language already exists", StatusCode = 400 };

            var language = _mapper.Map<Language>(dto);
            await _repository.AddAsync(language);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Language created successfully",
                StatusCode = 201
            };
        }

        public async Task<ResultDto> UpdateAsync(UpdateLanguageDto dto)
        {
            var language = await _repository.GetByIdAsync(dto.Id);
            if (language == null)
                throw new NotFoundException("Language not found");

            _mapper.Map(dto, language);
            _repository.Update(language);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Language updated successfully",
                StatusCode = 200
            };
        }

        public async Task<ResultDto> DeleteAsync(int id)
        {
            var language = await _repository.GetByIdAsync(id);
            if (language == null)
                throw new NotFoundException("Language not found");

            _repository.Delete(language);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Language deleted successfully",
                StatusCode = 200
            };
        }

        public async Task<List<LanguageDto>> GetAllAsync()
        {
            var languages = await _repository.GetAll(true).ToListAsync();
            return _mapper.Map<List<LanguageDto>>(languages);
        }

        public async Task<LanguageDto> GetByIdAsync(int id)
        {
            var language = await _repository.GetByIdAsync(id);
            if (language == null)
                throw new NotFoundException("Language not found");

            return _mapper.Map<LanguageDto>(language);
        }
    }
}
