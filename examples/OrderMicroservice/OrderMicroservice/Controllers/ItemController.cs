using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderMicroservice.Mediatr.Commands.CreateItemCommand;
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
                Username = request.Username,
                ItemType = request.ItemType
            });
            
            if(res != null)
                return Ok(res);
            return StatusCode(400, "Failed to create item");
        }
        
    }
}