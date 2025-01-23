using Application.Responses.Sale;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISaleRepository
    {
        Task<List<SaleResponse>> GetAllAsync();
        Task<SaleResponse> GetByIdAsync(int saleId);
        Task AddAsync(SaleEntity sale);
        Task UpdateAsync(SaleEntity sale);
    }
}
