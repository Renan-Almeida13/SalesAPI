using MediatR;

namespace Domain.Events
{
    public class SaleCreatedEvent : INotification
    {
        public int SaleId { get; }
        public DateTime SaleDate { get; }
        public decimal TotalAmount { get; }

        public SaleCreatedEvent(int saleId, DateTime saleDate, decimal totalAmount)
        {
            SaleId = saleId;
            SaleDate = saleDate;
            TotalAmount = totalAmount;
        }
    }
}
