using Domain.Commons;
using Domain.DTOs;
using MediatR;

namespace Application.Commands.Sale
{
    public class SaleCommand { }

    public class CreateSaleCommand : IRequest<Response>
    {
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public string Branch { get; set; }
        public List<CreateSaleItemDTO> Products { get; set; }

        public CreateSaleCommand()
        {
            Products = new List<CreateSaleItemDTO>();
        }
    }

    public class UpdateSaleCommand : IRequest<Response>
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public string Branch { get; set; }
        public List<UpdateSaleItemDTO> Products { get; set; }

        public UpdateSaleCommand()
        {
            Products = new List<UpdateSaleItemDTO>();
        }
    }

    public class DeleteSaleCommand : IRequest<Response>
    {
        public int SaleId { get; set; }

        public DeleteSaleCommand(int saleId)
        {
            SaleId = saleId;
        }
    }
}
