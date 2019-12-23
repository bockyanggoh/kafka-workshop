using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PaymentMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController: ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // [HttpGet("{orderId}")]
        // public async Task<IActionResult> GetOrderPaymentInformation([FromRoute]string orderId)
        // {
        //     
        // }
        
    }
}