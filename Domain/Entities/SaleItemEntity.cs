using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class SaleItemEntity
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }

        public int SaleId { get; set; }

        public SaleEntity Sale { get; set; }

        public bool IsCancelled { get; set; } = false;
    }

}
