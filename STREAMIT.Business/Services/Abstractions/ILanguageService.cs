using STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.LanguageDtos.STREAMIT.Business.Dtos.LanguageDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface ILanguageService
    {
        Task<ResultDto> CreateAsync(CreateLanguageDto dto);
        Task<ResultDto> UpdateAsync(UpdateLanguageDto dto);
        Task<ResultDto> DeleteAsync(int id);
        Task<List<LanguageDto>> GetAllAsync();
        Task<LanguageDto> GetByIdAsync(int id);
    }
}
