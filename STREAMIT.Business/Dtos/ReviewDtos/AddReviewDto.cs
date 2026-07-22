namespace STREAMIT.Business.Dtos.ReviewDtos
{
    public class AddReviewDto
    {
        public int MovieId { get; set; }
        public decimal Rating { get; set; }
        public string? Comment { get; set; }
        public string? UserId { get; set; } // set by MVC controller server-side
    }
}
