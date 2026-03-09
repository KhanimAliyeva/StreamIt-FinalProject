using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Dtos.PurchaseDtos
{
    public class PurchaseInfoVM
    {
        public int Id { get; set; }
        public string TypeRid { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PrevStatus { get; set; } = string.Empty;
        public string LastStatusLogin { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string CreateTime { get; set; } = string.Empty;
        public PurchaseTypeVM Type { get; set; }
    }

    public class PurchaseTypeVM
    {
        public string Title { get; set; } = string.Empty;
    }

    public class PurchaseDetailVM
    {
        public PurchaseInfoVM Order { get; set; } = null!;
    }
}

