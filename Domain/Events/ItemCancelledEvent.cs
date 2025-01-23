using MediatR;

namespace Domain.Events
{
    public class ItemCancelledEvent : INotification
    {
        public int SaleId { get; }
        public int ItemId { get; }
        public DateTime CancelledAt { get; }

        public ItemCancelledEvent(int saleId, int itemId, DateTime cancelledAt)
        {
            SaleId = saleId;
            ItemId = itemId;
            CancelledAt = cancelledAt;
        }
    }
}
