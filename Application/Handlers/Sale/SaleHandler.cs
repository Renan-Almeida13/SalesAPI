using Application.Commands.Sale;
using Application.Helpers;
using Application.Interfaces;
using Application.Queries.Sale;
using Application.Validation;
using Domain.Commons;
using Domain.DTOs;
using Domain.Entities;
using Domain.Events;
using MediatR;
using System.Net;

namespace Application.Handlers.Sale
{
    public class SaleHandler { }

    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Response>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMediator _mediator;

        public CreateSaleHandler(ISaleRepository saleRepository, IMediator mediator)
        {
            _saleRepository = saleRepository;
            _mediator = mediator;
        }

        public async Task<Response> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResponse = ProductValidation.ValidateProductQuantities(
                    request.Products,
                    p => p.Quantity,
                    p => p.ProductName
                );

                if (validationResponse != null)
                    return validationResponse;

                var products = request.Products.Select(product => new SaleItemEntity
                {
                    ProductName = product.ProductName,
                    Quantity = product.Quantity,
                    UnitPrice = product.UnitPrice,
                    Discount = DiscountCalculator.CalculateDiscount(product.Quantity, product.UnitPrice),
                    TotalAmount = (product.Quantity * product.UnitPrice) - DiscountCalculator.CalculateDiscount(product.Quantity, product.UnitPrice),
                    IsCancelled = false
                }).ToList();

                var sale = new SaleEntity
                {
                    SaleDate = request.SaleDate,
                    Customer = request.Customer,
                    Branch = request.Branch,
                    Products = products,
                    TotalSaleAmount = products.Sum(p => p.TotalAmount),
                    IsCancelled = false
                };

                await _saleRepository.AddAsync(sale);

                var saleCreatedEvent = new SaleCreatedEvent(sale.Id, sale.SaleDate, sale.TotalSaleAmount);
                await _mediator.Publish(saleCreatedEvent);

                var saleDTO = new SaleDTO
                {
                    Id = sale.Id,
                    SaleDate = sale.SaleDate,
                    Customer = sale.Customer,
                    TotalSaleAmount = sale.TotalSaleAmount,
                    Branch = sale.Branch,
                    Products = sale.Products.Select(p => new SaleItemDTO
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice,
                        Discount = p.Discount,
                        TotalAmount = p.TotalAmount,
                        IsCancelled = p.IsCancelled
                    }).ToList()
                };

                return new Response(HttpStatusCode.OK, null, saleDTO);
            }
            catch (Exception ex)
            {
                return new Response(HttpStatusCode.InternalServerError, new[] { $"An error occurred: {ex.Message}" });
            }
        }
    }

    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Response>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMediator _mediator;

        public UpdateSaleHandler(ISaleRepository saleRepository, IMediator mediator)
        {
            _saleRepository = saleRepository;
            _mediator = mediator;
        }

        public async Task<Response> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var saleResponse = await _saleRepository.GetByIdAsync(request.SaleId);
                if (saleResponse == null)
                    return new Response(HttpStatusCode.NotFound, new[] { "Sale not found." });

                var existingSale = new SaleEntity
                {
                    Id = saleResponse.Id,
                    SaleDate = saleResponse.SaleDate,
                    Customer = saleResponse.Customer,
                    Branch = saleResponse.Branch,
                    Products = saleResponse.Products.Select(p => new SaleItemEntity
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice,
                        Discount = p.Discount,
                        TotalAmount = p.TotalAmount,
                        IsCancelled = p.IsCancelled
                    }).ToList()
                };

                var validationResponse = ProductValidation.ValidateProductQuantities(
                    request.Products,
                    p => p.Quantity,
                    p => p.ProductName
                );

                if (validationResponse != null)
                    return validationResponse;

                existingSale.SaleDate = request.SaleDate;
                existingSale.Customer = request.Customer;
                existingSale.Branch = request.Branch;

                var publishTasks = new List<Task>();
                foreach (var product in request.Products)
                {
                    if (product.Id.HasValue)
                    {
                        var existingProduct = existingSale.Products.FirstOrDefault(p => p.Id == product.Id.Value);
                        if (existingProduct != null)
                        {
                            UpdateExistingProduct(existingProduct, product);
                            if (existingProduct.IsCancelled)
                            {
                                var itemCancelledEvent = new ItemCancelledEvent(existingProduct.Id, existingSale.Id, DateTime.Now);
                                publishTasks.Add(_mediator.Publish(itemCancelledEvent));
                            }
                        }
                    }
                    else
                    {
                        AddNewProduct(existingSale, product);
                    }
                }
                await Task.WhenAll(publishTasks);

                existingSale.TotalSaleAmount = existingSale.Products.Where(p => !p.IsCancelled).Sum(p => p.TotalAmount);
                await _saleRepository.UpdateAsync(existingSale);

                var saleModifiedEvent = new SaleModifiedEvent(existingSale.Id, DateTime.Now, existingSale.TotalSaleAmount);
                await _mediator.Publish(saleModifiedEvent);

                var saleDTO = new SaleDTO
                {
                    Id = existingSale.Id,
                    SaleDate = existingSale.SaleDate,
                    Customer = existingSale.Customer,
                    TotalSaleAmount = existingSale.TotalSaleAmount,
                    Branch = existingSale.Branch,
                    Products = existingSale.Products.Select(p => new SaleItemDTO
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice,
                        Discount = p.Discount,
                        TotalAmount = p.TotalAmount,
                        IsCancelled = p.IsCancelled
                    }).ToList()
                };

                return new Response(HttpStatusCode.OK, null, saleDTO);
            }
            catch (Exception ex)
            {
                return new Response(HttpStatusCode.InternalServerError, new[] { ex.Message });
            }
        }

        private void UpdateExistingProduct(SaleItemEntity existingProduct, UpdateSaleItemDTO product)
        {
            existingProduct.ProductName = product.ProductName;
            existingProduct.Quantity = product.Quantity;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.Discount = DiscountCalculator.CalculateDiscount(product.Quantity, product.UnitPrice);
            existingProduct.TotalAmount = (product.Quantity * product.UnitPrice) - existingProduct.Discount;
            existingProduct.IsCancelled = product.IsCancelled;
        }

        private void AddNewProduct(SaleEntity existingSale, UpdateSaleItemDTO product)
        {
            var newProduct = new SaleItemEntity
            {
                ProductName = product.ProductName,
                Quantity = product.Quantity,
                UnitPrice = product.UnitPrice,
                Discount = DiscountCalculator.CalculateDiscount(product.Quantity, product.UnitPrice),
                TotalAmount = (product.Quantity * product.UnitPrice) - DiscountCalculator.CalculateDiscount(product.Quantity, product.UnitPrice),
                IsCancelled = false
            };
            existingSale.Products.Add(newProduct);
        }
    }

    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Response>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMediator _mediator;

        public DeleteSaleHandler(ISaleRepository saleRepository, IMediator mediator)
        {
            _saleRepository = saleRepository;
            _mediator = mediator;
        }

        public async Task<Response> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var saleResponse = await _saleRepository.GetByIdAsync(request.SaleId);
                if (saleResponse == null)
                    return new Response(HttpStatusCode.NotFound, new[] { "Sale not found." });

                var existingSale = new SaleEntity
                {
                    Id = saleResponse.Id,
                    SaleDate = saleResponse.SaleDate,
                    Customer = saleResponse.Customer,
                    Branch = saleResponse.Branch,
                    Products = saleResponse.Products.Select(p => new SaleItemEntity
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice,
                        Discount = p.Discount,
                        TotalAmount = p.TotalAmount,
                        IsCancelled = p.IsCancelled
                    }).ToList()
                };

                CancelSale(existingSale);

                await _saleRepository.UpdateAsync(existingSale);

                var saleCancelledEvent = new SaleCancelledEvent(existingSale.Id, DateTime.Now);
                await _mediator.Publish(saleCancelledEvent);

                var saleDTO = new SaleDTO
                {
                    Id = existingSale.Id,
                    SaleDate = existingSale.SaleDate
                };

                return new Response(HttpStatusCode.OK, null, saleDTO);
            }
            catch (Exception ex)
            {
                return new Response(HttpStatusCode.InternalServerError, new[] { ex.Message });
            }
        }

        private void CancelSale(SaleEntity sale)
        {
            sale.IsCancelled = true;
            sale.TotalSaleAmount = 0;

            foreach (var item in sale.Products)
            {
                item.IsCancelled = true;
                item.UnitPrice = 0;
                item.Quantity = 0;
                item.Discount = 0;
                item.TotalAmount = 0;
            }
        }
    }

    public class GetAllSalesHandler : IRequestHandler<GetAllSalesQuery, Response>
    {
        private readonly ISaleRepository _saleRepository;

        public GetAllSalesHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<Response> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sales = await _saleRepository.GetAllAsync();

                if (sales == null || !sales.Any())
                    return new Response(HttpStatusCode.NotFound, new[] { "No sales found." });

                return new Response(HttpStatusCode.OK, data: sales);
            }
            catch (Exception ex)
            {
                return new Response(HttpStatusCode.InternalServerError, new[] { ex.Message });
            }
        }
    }

    public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, Response>
    {
        private readonly ISaleRepository _saleRepository;

        public GetSaleByIdHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<Response> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(request.SaleId);

                if (sale == null)
                    return new Response(HttpStatusCode.NotFound, new[] { "Sale not found." });

                return new Response(HttpStatusCode.OK, data: sale);
            }
            catch (Exception ex)
            {
                return new Response(HttpStatusCode.InternalServerError, new[] { ex.Message });
            }
        }
    }
}
