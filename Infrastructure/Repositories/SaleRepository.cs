using Application.Interfaces;
using Application.Responses.Sale;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _dbContext;

        public SaleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SaleResponse>> GetAllAsync()
        {
            return await _dbContext.Sales
                .AsNoTracking()
                .Where(s => !s.IsCancelled)
                .Select(s => new SaleResponse
                {
                    Id = s.Id,
                    SaleDate = s.SaleDate,
                    Customer = s.Customer,
                    TotalSaleAmount = s.TotalSaleAmount,
                    Branch = s.Branch,
                    Products = s.Products
                        .Where(p => !p.IsCancelled)
                        .Select(p => new SaleItemResponse
                        {
                            Id = p.Id,
                            ProductName = p.ProductName,
                            Quantity = p.Quantity,
                            UnitPrice = p.UnitPrice,
                            Discount = p.Discount,
                            TotalAmount = p.TotalAmount
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<SaleResponse> GetByIdAsync(int saleId)
        {
            return await _dbContext.Sales
                .AsNoTracking()
                .Where(s => s.Id == saleId && !s.IsCancelled) // Filtro para a venda pelo ID e não cancelada
                .Select(s => new SaleResponse
                {
                    Id = s.Id,
                    SaleDate = s.SaleDate,
                    Customer = s.Customer,
                    TotalSaleAmount = s.TotalSaleAmount,
                    Branch = s.Branch,
                    Products = s.Products
                        .Where(p => !p.IsCancelled) // Apenas produtos não cancelados
                        .Select(p => new SaleItemResponse
                        {
                            Id = p.Id,
                            ProductName = p.ProductName,
                            Quantity = p.Quantity,
                            UnitPrice = p.UnitPrice,
                            Discount = p.Discount,
                            TotalAmount = p.TotalAmount
                        }).ToList()
                })
                .FirstOrDefaultAsync(); // Busca o primeiro registro ou retorna null se não encontrado
        }

        public async Task AddAsync(SaleEntity sale)
        {
            await _dbContext.Sales.AddAsync(sale);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SaleEntity sale)
        {
            _dbContext.Sales.Update(sale);
            await _dbContext.SaveChangesAsync();
        }
    }
}
