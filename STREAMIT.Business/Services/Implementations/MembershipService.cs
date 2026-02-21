using Microsoft.EntityFrameworkCore;
using STREAMIT.Business.Dtos.MembershipDtos;
using STREAMIT.Business.Dtos.ResultDtos;
using STREAMIT.Business.Services.Abstractions;
using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;

namespace STREAMIT.Business.Services.Implementations;

public class MembershipService(AppDbContext context) : IMembershipService
{
    #region GET ALL

    public async Task<ResultDto<List<GetMembershipDto>>> GetAllAsync()
    {
        var memberships = await context.Memberships
            .Where(x => !x.IsDeleted)
            .Select(x => new GetMembershipDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                DurationInDays = x.DurationInDays,
                VideoQuality = x.VideoQuality,
                MaxDevices = x.MaxDevices,
                HasAds = x.HasAds,
                CanDownload = x.CanDownload,
                IsActive = x.IsActive,
                PriorityLevel = x.PriorityLevel
            })
            .OrderBy(x => x.PriorityLevel)
            .ToListAsync();

        return new ResultDto<List<GetMembershipDto>>
        {
            IsSucceed = true,
            Data = memberships
        };
    }

    #endregion


    #region GET BY ID

    public async Task<ResultDto<GetMembershipDto>> GetByIdAsync(int id)
    {
        var membership = await context.Memberships
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new GetMembershipDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                DurationInDays = x.DurationInDays,
                VideoQuality = x.VideoQuality,
                MaxDevices = x.MaxDevices,
                HasAds = x.HasAds,
                CanDownload = x.CanDownload,
                IsActive = x.IsActive,
                PriorityLevel = x.PriorityLevel
            })
            .FirstOrDefaultAsync();

        if (membership is null)
            return new ResultDto<GetMembershipDto>
            {
                IsSucceed = false,
                Message = "Membership not found"
            };

        return new ResultDto<GetMembershipDto>
        {
            IsSucceed = true,
            Data = membership
        };
    }

    #endregion


    #region CREATE

    public async Task<ResultDto> CreateAsync(CreateMembershipDto dto)
    {
        var membership = new Membership
        {
            Name = dto.Name,
            Price = dto.Price,
            DurationInDays = dto.DurationInDays,
            VideoQuality = dto.VideoQuality,
            MaxDevices = dto.MaxDevices,
            HasAds = dto.HasAds,
            CanDownload = dto.CanDownload,
            IsActive = dto.IsActive,
            PriorityLevel = dto.PriorityLevel
        };

        await context.Memberships.AddAsync(membership);
        await context.SaveChangesAsync();

        return new ResultDto
        {
            IsSucceed = true,
            Message = "Membership created successfully"
        };
    }

    #endregion


    #region UPDATE

    public async Task<ResultDto> UpdateAsync(UpdateMembershipDto dto)
    {
        var membership = await context.Memberships
            .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted);

        if (membership is null)
            return new ResultDto
            {
                IsSucceed = false,
                Message = "Membership not found"
            };

        membership.Name = dto.Name;
        membership.Price = dto.Price;
        membership.DurationInDays = dto.DurationInDays;
        membership.VideoQuality = dto.VideoQuality;
        membership.MaxDevices = dto.MaxDevices;
        membership.HasAds = dto.HasAds;
        membership.CanDownload = dto.CanDownload;
        membership.IsActive = dto.IsActive;
        membership.PriorityLevel = dto.PriorityLevel;

        context.Memberships.Update(membership);
        await context.SaveChangesAsync();

        return new ResultDto
        {
            IsSucceed = true,
            Message = "Membership updated successfully"
        };
    }

    #endregion


    #region DELETE (Soft Delete)

    public async Task<ResultDto> DeleteAsync(int id)
    {
        var membership = await context.Memberships
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (membership is null)
            return new ResultDto
            {
                IsSucceed = false,
                Message = "Membership not found"
            };

        membership.IsDeleted = true;

        context.Memberships.Update(membership);
        await context.SaveChangesAsync();

        return new ResultDto
        {
            IsSucceed = true,
            Message = "Membership deleted successfully"
        };
    }

    #endregion
}
