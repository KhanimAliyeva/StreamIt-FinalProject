using AutoMapper;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Dtos.TagDtos;
using STREAMIT.Business.Exceptions;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Repositories.Abstractions;
using STREAMIT.DataAccess.Repositories.Abstractions.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STREAMIT.Business.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag> _repository;
        private readonly IMapper _mapper;

        public TagService(IRepository<Tag> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultDto> CreateAsync(CreateTagDto dto)
        {
            var exists = await _repository.AnyAsync(x => x.Name == dto.Name);
            if (exists)
                return new ResultDto { IsSucceed = false, Message = "Tag already exists", StatusCode = 400 };

            var tag = _mapper.Map<Tag>(dto);
            await _repository.AddAsync(tag);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Tag created successfully",
                StatusCode = 201
            };
        }

        public async Task<ResultDto> UpdateAsync(UpdateTagDto dto)
        {
            var tag = await _repository.GetByIdAsync(dto.Id);
            if (tag == null)
                throw new NotFoundException("Tag not found");

            _mapper.Map(dto, tag);
            _repository.Update(tag);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Tag updated successfully",
                StatusCode = 200
            };
        }

        public async Task<ResultDto> DeleteAsync(int id)
        {
            var tag = await _repository.GetByIdAsync(id);
            if (tag == null)
                throw new NotFoundException("Tag not found");

            _repository.Delete(tag);
            await _repository.SaveChangesAsync();

            return new ResultDto
            {
                IsSucceed = true,
                Message = "Tag deleted successfully",
                StatusCode = 200
            };
        }

        public async Task<List<GetTagDto>> GetAllAsync()
        {
            var tags = await _repository.GetAll(true).ToListAsync();
            return _mapper.Map<List<GetTagDto>>(tags);
        }

        public async Task<GetTagDto> GetByIdAsync(int id)
        {
            var tag = await _repository.GetByIdAsync(id);
            if (tag == null)
                throw new NotFoundException("Tag not found");

            return _mapper.Map<GetTagDto>(tag);
        }
    }
}
