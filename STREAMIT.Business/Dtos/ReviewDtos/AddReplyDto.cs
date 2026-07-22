namespace STREAMIT.Business.Dtos.ReviewDtos
{
    public class AddReplyDto
    {
        public int MovieId { get; set; }
        public int ParentReviewId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
