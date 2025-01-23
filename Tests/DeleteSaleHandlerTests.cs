using Application.Commands.Sale;
using Application.Handlers.Sale;
using Application.Interfaces;
using Application.Responses.Sale;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Moq;
using System.Net;

namespace SaleTest
{
    public class DeleteSaleHandlerTests
    {
        private readonly Mock<ISaleRepository> _saleRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DeleteSaleHandler _deleteSaleHandler;

        public DeleteSaleHandlerTests()
        {
            _saleRepositoryMock = new Mock<ISaleRepository>();
            _mediatorMock = new Mock<IMediator>();
            _deleteSaleHandler = new DeleteSaleHandler(_saleRepositoryMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_SaleExists_ShouldMarkSaleAndItemsAsCancelled()
        {
            int saleId = 123;
            var saleResponse = new SaleResponse
            {
                Id = saleId,
                SaleDate = DateTime.Now,
                Customer = "CustomerName",
                TotalSaleAmount = 100,
                Branch = "BranchName",
                Products = new List<SaleItemResponse>
        {
            new SaleItemResponse { Id = 1, ProductName = "Product1", IsCancelled = false, Quantity = 2, UnitPrice = 50, Discount = 0, TotalAmount = 100 },
            new SaleItemResponse { Id = 2, ProductName = "Product2", IsCancelled = false, Quantity = 3, UnitPrice = 30, Discount = 0, TotalAmount = 90 }
        }
            };

            _saleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(saleResponse);

            var response = await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Sale successfully deleted.", response.Errors);

            Assert.Equal(0, saleResponse.TotalSaleAmount);
            Assert.All(saleResponse.Products, item =>
            {
                Assert.True(item.IsCancelled);
                Assert.Equal(0, item.UnitPrice);
                Assert.Equal(0, item.Quantity);
                Assert.Equal(0, item.Discount);
                Assert.Equal(0, item.TotalAmount);
            });

            _mediatorMock.Verify(m => m.Publish(It.IsAny<SaleCancelledEvent>(), CancellationToken.None), Times.Once);
            _saleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SaleEntity>()), Times.Once);
        }

        [Fact]
        public async Task Handle_SaleNotFound_ShouldReturnNotFound()
        {
            int saleId = 123;
            _saleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((SaleResponse)null);

            var response = await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Sale not found.", response.Errors);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<SaleCancelledEvent>(), CancellationToken.None), Times.Never);
        }


        [Fact]
        public async Task Handle_Exception_ShouldReturnInternalServerError()
        {
            int saleId = 123;
            var sale = new SaleEntity { Id = saleId };
            _saleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).Throws(new Exception("An error occurred"));

            var response = await _deleteSaleHandler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("An error occurred", response.Errors);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<SaleCancelledEvent>(), CancellationToken.None), Times.Never);
        }
    }
}
