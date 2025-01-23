using Domain.Commons;
using MediatR;

namespace Application.Responses.Sale
{
    public class SalesResponse { }

    public class SaleResponse : IRequest<Response>
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public decimal TotalSaleAmount { get; set; }
        public string Branch { get; set; }
        public List<SaleItemResponse> Products { get; set; }
    }

    public class SaleItemResponse : IRequest<Response>
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
