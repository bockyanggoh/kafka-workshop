using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.Mediatr.Commands.CreateItemCommand;
using OrderMicroservice.Mediatr.Commands.CreateItemsCommand;
using OrderMicroservice.RequestModel;

namespace OrderMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut]
        public async Task<IActionResult> CreateItemsApi([FromBody] CreateItemsRequest request)
        {
            var res = await _mediator.Send(new CreateItemsCommand
            {
                Request = request
            });
            
            if(res.RequestStatus == CustomEnum.RequestStatus.Success)
                return Ok(res);

            return BadRequest(res);
        }
    }
}