using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.Models.ResponseModel;
using PaymentMicroservice.Mediatr.Queries;
using PaymentMicroservice.Mediatr.Queries.FindPaymentQuery;

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

        [HttpGet("")]
        public async Task<IActionResult> GetOrderPaymentInformation([FromQuery]string orderId="default", string paymentId="default")
        {
            if (orderId == "default" && paymentId == "default")
            {
                return StatusCode(400,
                    new StandardResponse("Either orderId or paymentId must be provided. Please try again."));
            }

            if (orderId == "default")
            {
                var res = await _mediator.Send(new FindPaymentQuery
                {
                    PaymentId = paymentId
                });
                if(res != null)
                    return StatusCode(200, res);

                return StatusCode(400, new StandardResponse($"No payment information for PaymentId {paymentId} was found."));
            }
            else
            {
                var res = await _mediator.Send(new FindPaymentQuery
                {
                    OrderId = orderId
                });
                if(res != null)
                    return StatusCode(200, res);

                return StatusCode(400, new StandardResponse($"No payment information for OrderId {paymentId} was found."));
            }
        }
        
    }
}