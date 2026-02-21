using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface IMembershipService
    {
        Task<ResultDto<List<GetMembershipDto>>> GetAllAsync();
        Task<ResultDto<GetMembershipDto>> GetByIdAsync(int id);
        Task<ResultDto> CreateAsync(CreateMembershipDto dto);
        Task<ResultDto> UpdateAsync(UpdateMembershipDto dto);
        Task<ResultDto> DeleteAsync(int id);
    }
}
