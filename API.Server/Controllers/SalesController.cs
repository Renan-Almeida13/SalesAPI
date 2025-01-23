using Application.Commands.Sale;
using Application.Queries.Sale;
using Domain.Commons;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSaleAsync([FromBody] CreateSaleCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _mediator.Send(command);
            return HandleApiResponse(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesAsync()
        {
            var query = new GetAllSalesQuery();
            var response = await _mediator.Send(query);
            return HandleApiResponse(response);
        }

        [HttpGet("{saleId}")]
        public async Task<IActionResult> GetSaleByIdAsync(int saleId)
        {
            var query = new GetSaleByIdQuery(saleId);
            var response = await _mediator.Send(query);
            return HandleApiResponse(response);
        }

        [HttpPut("{saleId}")]
        public async Task<IActionResult> UpdateSaleAsync(int saleId, [FromBody] UpdateSaleCommand command)
        {
            if (saleId != command.SaleId)
                return BadRequest(new { message = "Sale ID in the route does not match the command data." });

            var response = await _mediator.Send(command);
            return HandleApiResponse(response);
        }

        [HttpDelete("{saleId}")]
        public async Task<IActionResult> DeleteSaleAsync(int saleId)
        {
            var command = new DeleteSaleCommand(saleId);
            var response = await _mediator.Send(command);
            return HandleApiResponse(response);
        }

        private IActionResult HandleApiResponse(Response response)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.OK => Ok(response.Data),
                HttpStatusCode.NotFound => NotFound(response.Errors),
                HttpStatusCode.BadRequest => BadRequest(response.Errors),
                HttpStatusCode.Unauthorized => Unauthorized(new { message = "Authentication failed or user not authorized." }),
                HttpStatusCode.Forbidden => Forbid(),
                HttpStatusCode.InternalServerError => StatusCode(500, response.Errors), // Mapeamento explícito para 500
                _ => StatusCode((int)response.StatusCode, response.Errors) // Caso genérico
            };
        }
    }
}
