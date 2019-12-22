using System.Threading.Tasks;
using Kafka.Communication.Models;
using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.RequestModel;
using OrderMicroservice.Services.Publisher;

namespace OrderMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly KafkaOrdersService _ordersService;
        public OrderController(KafkaOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost("/order")]
        public async Task<IActionResult> CreateOrderApi([FromBody] OrderUserRequest request)
        {
            var res = await _ordersService.CreatePaymentRequest(request);
            return res.Success ? Ok(res) : StatusCode(500, res);
        }

        [HttpGet("/order/status/{id}")]
        public async Task<IActionResult> CheckOrderStatusApi()
        {
            return Ok("Placeholder");
        }
        
    }
}