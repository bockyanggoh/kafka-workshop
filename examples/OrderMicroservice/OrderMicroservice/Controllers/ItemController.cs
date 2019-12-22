using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderMicroservice.Mediatr.Commands.CreateItemCommand;
using OrderMicroservice.Mediatr.Queries.GetItemQuery;
using OrderMicroservice.Mediatr.Queries.GetItemsQuery;
using OrderMicroservice.RequestModel;

namespace OrderMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("single")]
        public async Task<IActionResult> CreateSingleItemApi([FromBody]CreateItemRequest request)
        {
            var res = await _mediator.Send(new CreateItemCommand
            {
                ItemName = request.ItemName,
                ItemType = request.ItemType
            });
            
            if(res != null)
                return Ok(res);
            return StatusCode(400, "Failed to create item");
        }

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetItemsApi()
        {
            var res = await _mediator.Send(new GetItemsQuery());
            if (res != null)
            {
                return Ok(res);
            }

            return StatusCode(500, "Failed to find data");
        }

        [HttpGet("{itemId}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetItemApi([FromRoute]string itemId)
        {
            var res = await _mediator.Send(new GetItemQuery()
            {
                ItemId = itemId
            });

            if (res != null)
            {
                return Ok(res);
            }
            return StatusCode(500, "Failed to find data");
        }
    }
}