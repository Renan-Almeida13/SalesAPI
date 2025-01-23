using Application.Commands.Sale;
using Application.Handlers.Sale;
using Application.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Domain.Events;
using MediatR;
using Moq;
using System.Net;

namespace SaleTest
{
    public class CreateSaleHandlerTests
    {
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly CreateSaleHandler _handler;

        public CreateSaleHandlerTests()
        {
            _mockSaleRepository = new Mock<ISaleRepository>();
            _mockMediator = new Mock<IMediator>();
            _handler = new CreateSaleHandler(_mockSaleRepository.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_CreateSale_Success()
        {
            var request = new CreateSaleCommand
            {
                SaleDate = DateTime.Now,
                Customer = "Customer1",
                Branch = "Branch1",
                Products = new List<CreateSaleItemDTO>
            {
                new CreateSaleItemDTO
                {
                    ProductName = "Product1",
                    Quantity = 5,
                    UnitPrice = 10m
                }
            }
            };

            _mockSaleRepository.Setup(repo => repo.AddAsync(It.IsAny<SaleEntity>()))
                               .Returns(Task.CompletedTask);

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Contains("Sale and associated items successfully created.", response.Errors);
            _mockMediator.Verify(mediator => mediator.Publish(It.IsAny<SaleCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CreateSale_QuantityValidation_Fails()
        {
            var request = new CreateSaleCommand
            {
                SaleDate = DateTime.Now,
                Customer = "Customer1",
                Branch = "Branch1",
                Products = new List<CreateSaleItemDTO>
            {
                new CreateSaleItemDTO
                {
                    ProductName = "Product1",
                    Quantity = 25,
                    UnitPrice = 10m
                }
            }
            };

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Cannot sell more than 20 identical items.", response.Errors);
        }

        [Fact]
        public async Task Handle_CreateSale_Exception_ReturnsInternalServerError()
        {
            var request = new CreateSaleCommand
            {
                SaleDate = DateTime.Now,
                Customer = "Customer1",
                Branch = "Branch1",
                Products = new List<CreateSaleItemDTO>
            {
                new CreateSaleItemDTO
                {
                    ProductName = "Product1",
                    Quantity = 5,
                    UnitPrice = 10m
                }
            }
            };

            _mockSaleRepository.Setup(repo => repo.AddAsync(It.IsAny<SaleEntity>()))
                               .ThrowsAsync(new Exception("Database error"));

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Contains("An error occurred: Database error", response.Errors);
        }
    }
}
