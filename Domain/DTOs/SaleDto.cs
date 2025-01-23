namespace Domain.DTOs
{
    public class CreateSaleItemDTO
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class UpdateSaleItemDTO
    {
        public int? Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsCancelled { get; set; }
    }

    public class SaleDTO
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public decimal TotalSaleAmount { get; set; }
        public string Branch { get; set; }
        public List<SaleItemDTO> Products { get; set; }
    }

    public class SaleItemDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
    }
}
