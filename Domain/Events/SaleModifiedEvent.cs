using MediatR;

namespace Domain.Events
{
    public class SaleModifiedEvent : INotification
    {
        public int SaleId { get; }
        public DateTime ModifiedDate { get; }
        public decimal NewTotalAmount { get; }

        public SaleModifiedEvent(int saleId, DateTime modifiedDate, decimal newTotalAmount)
        {
            SaleId = saleId;
            ModifiedDate = modifiedDate;
            NewTotalAmount = newTotalAmount;
        }
    }
}
