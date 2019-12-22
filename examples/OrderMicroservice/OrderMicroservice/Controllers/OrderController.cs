using System.Threading.Tasks;
using Kafka.Communication.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.Mediatr.Commands.CreateOrderCommand;
using OrderMicroservice.RequestModel;
using OrderMicroservice.Services.Publisher;

namespace OrderMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly KafkaOrdersService _ordersService;
        public OrderController(IMediator mediatr, KafkaOrdersService ordersService)
        {
            _ordersService = ordersService;
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