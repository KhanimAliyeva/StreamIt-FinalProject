using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class KapitalBankCallbackModel
    {
        public string OrderId { get; set; } = string.Empty;    // Sənin OrderId / Payment.OrderId ilə match edəcək
        public string Status { get; set; } = string.Empty;     // success / failed / pending
        public decimal Amount { get; set; }                    // ödənilən məbləğ
        public string Currency { get; set; } = string.Empty;   // AZN
        public string Description { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;  // Kapital Bank-un öz id-si
    }
}
