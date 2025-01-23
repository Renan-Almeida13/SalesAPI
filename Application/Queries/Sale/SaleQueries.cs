using Domain.Commons;
using MediatR;

namespace Application.Queries.Sale
{
    public class SaleQueries { }

    public class GetAllSalesQuery : IRequest<Response> { }

    public class GetSaleByIdQuery : IRequest<Response>
    {
        public int SaleId { get; set; }

        public GetSaleByIdQuery(int saleId)
        {
            SaleId = saleId;
        }
    }
}
