using Application.Commands.Sale;
using Application.Handlers.Sale;
using Application.Interfaces;
using Application.Responses.Sale;
using Domain.DTOs;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Moq;
using System.Net;

namespace SaleTest
{
    public class UpdateSaleHandlerTests
    {
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly UpdateSaleHandler _handler;

        public UpdateSaleHandlerTests()
        {
            _mockSaleRepository = new Mock<ISaleRepository>();
            _mockMediator = new Mock<IMediator>();
            _handler = new UpdateSaleHandler(_mockSaleRepository.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_UpdateSale_Success()
        {
            var saleId = 1;
            var existingSale = new SaleResponse
            {
                Id = saleId,
                SaleDate = DateTime.Now.AddDays(-1),
                Customer = "Customer1",
                Branch = "Branch1",
                Products = new List<SaleItemResponse>
        {
            new SaleItemResponse
            {
                Id = 1,
                ProductName = "Product1",
                Quantity = 5,
                UnitPrice = 10m,
                Discount = 0,
                TotalAmount = 50m,
                IsCancelled = false
            }
        },
                TotalSaleAmount = 50m
            };

            var request = new UpdateSaleCommand
            {
                SaleId = saleId,
                SaleDate = DateTime.Now,
                Customer = "UpdatedCustomer",
                Branch = "UpdatedBranch",
                Products = new List<UpdateSaleItemDTO>
        {
            new UpdateSaleItemDTO
            {
                Id = 1,
                ProductName = "UpdatedProduct1",
                Quantity = 10,
                UnitPrice = 8m,
                IsCancelled = false
            }
        }
            };

            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId))
                               .ReturnsAsync(existingSale);

            _mockSaleRepository.Setup(repo => repo.UpdateAsync(It.IsAny<SaleEntity>()))
                               .Returns(Task.CompletedTask);

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Sale successfully updated.", response.Errors);
            Assert.Equal("UpdatedCustomer", existingSale.Customer);
            Assert.Equal("UpdatedBranch", existingSale.Branch);
            _mockMediator.Verify(mediator => mediator.Publish(It.IsAny<SaleModifiedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateSale_SaleNotFound()
        {
            var saleId = 1;
            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId))
                               .ReturnsAsync((SaleResponse)null); // Sale não encontrada

            var request = new UpdateSaleCommand
            {
                SaleId = saleId,
                SaleDate = DateTime.Now,
                Customer = "UpdatedCustomer",
                Branch = "UpdatedBranch",
                Products = new List<UpdateSaleItemDTO>
        {
            new UpdateSaleItemDTO
            {
                ProductName = "UpdatedProduct1",
                Quantity = 10,
                UnitPrice = 8m
            }
        }
            };

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Sale not found.", response.Errors);
        }

        [Fact]
        public async Task Handle_UpdateSale_QuantityValidation_Fails()
        {
            var saleId = 1;
            var existingSale = new SaleResponse
            {
                Id = saleId,
                Products = new List<SaleItemResponse>
            {
            new SaleItemResponse { Id = 1, ProductName = "Product1", Quantity = 5, UnitPrice = 10m, Discount = 0, TotalAmount = 50m, IsCancelled = false }
            },
                TotalSaleAmount = 50m
            };

            var request = new UpdateSaleCommand
            {
                SaleId = saleId,
                Products = new List<UpdateSaleItemDTO>
        {
            new UpdateSaleItemDTO
            {
                ProductName = "Product1",
                Quantity = 25, // Quantidade inválida
                UnitPrice = 10m
            }
        }
            };

            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId))
                               .ReturnsAsync(existingSale);

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Cannot sell more than 20 identical items.", response.Errors);
        }

        [Fact]
        public async Task Handle_UpdateSale_Exception_ReturnsInternalServerError()
        {
            var saleId = 1;
            var existingSale = new SaleResponse
            {
                Id = saleId,
                Products = new List<SaleItemResponse>
        {
            new SaleItemResponse { Id = 1, ProductName = "Product1", Quantity = 5, UnitPrice = 10m, Discount = 0, TotalAmount = 50m, IsCancelled = false }
        },
                TotalSaleAmount = 50m
            };

            var request = new UpdateSaleCommand
            {
                SaleId = saleId,
                Products = new List<UpdateSaleItemDTO>
        {
            new UpdateSaleItemDTO
            {
                ProductName = "Product1",
                Quantity = 5,
                UnitPrice = 10m
            }
        }
            };

            _mockSaleRepository.Setup(repo => repo.GetByIdAsync(saleId))
                               .ReturnsAsync(existingSale);

            _mockSaleRepository.Setup(repo => repo.UpdateAsync(It.IsAny<SaleEntity>()))
                               .ThrowsAsync(new Exception("Database error"));

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("An error occurred: Database error", response.Errors);
        }

    }
}
