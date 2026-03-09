using STREAMIT.Business.Dtos.PaymentDtos;
using STREAMIT.Business.Dtos.PurchaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Business.Services.Abstractions
{
    public interface IPaymentService
    {
        Task<PurchaseVM> CreatePaymentAsync(MembershipCreateVM vm);
        Task<PurchaseDetailVM> GetPaymentInfoAsync(int Id);
    }
}
