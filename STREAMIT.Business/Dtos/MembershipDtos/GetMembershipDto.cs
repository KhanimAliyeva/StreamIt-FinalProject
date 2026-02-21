namespace STREAMIT.Business.Dtos.MembershipDtos;

public class GetMembershipDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int DurationInDays { get; set; }

    public string VideoQuality { get; set; } = string.Empty;
    public int MaxDevices { get; set; }

    public bool HasAds { get; set; }
    public bool CanDownload { get; set; }
    public bool IsActive { get; set; }

    public int PriorityLevel { get; set; }
}
