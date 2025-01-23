using MediatR;

namespace Domain.Events
{
    public class SaleCancelledEvent : INotification
    {
        public int SaleId { get; }
        public DateTime CancelledDate { get; }

        public SaleCancelledEvent(int saleId, DateTime cancelledDate)
        {
            SaleId = saleId;
            CancelledDate = cancelledDate;
        }
    }
}
