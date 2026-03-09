namespace STREAMIT.Business.Dtos.PaymentDtos
{
    public class MembershipCreateVM
    {
        public int Amount { get; set; }
        public string Currency { get; set; } = "AZN";
        public string Description { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }
}
