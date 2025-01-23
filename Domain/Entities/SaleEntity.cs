using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class SaleEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public decimal TotalSaleAmount { get; set; }
        public string Branch { get; set; }
        public List<SaleItemEntity> Products { get; set; }

        public bool IsCancelled { get; set; }

        public SaleEntity()
        {
            Products = new List<SaleItemEntity>();
        }
    }
}
