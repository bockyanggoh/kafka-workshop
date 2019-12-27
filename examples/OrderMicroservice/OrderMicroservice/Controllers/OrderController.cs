using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.Mediatr.Commands.CreateOrderCommand;
using OrderMicroservice.Models.CustomEnum;
using OrderMicroservice.RequestModel;

namespace OrderMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediatr;
        public OrderController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        [HttpPut]
        public async Task<IActionResult> CreateOrderApi([FromBody] OrderUserRequest request)
        {
            var res = await _mediatr.Send(new CreateOrderCommand
            {
                Username = request.Username,
                OrderItemIds = request.OrderIds
            });
            if (res.RequestStatus == CustomEnum.RequestStatus.Success)
                return Ok(res);
            return StatusCode(500, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> CheckOrderStatusApi()
        {
            return Ok("Placeholder");
        }
        
    }
}