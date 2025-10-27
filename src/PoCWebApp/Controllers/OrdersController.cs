using MediatR;
using Microsoft.AspNetCore.Mvc;
using PoCWebApp.Applications.Commands;

namespace PoCWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { OrderId = id });
        }
    }

}
