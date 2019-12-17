using System;
using System.Threading.Tasks;
using KafkaPublisherContainer.RequestModel;
using KafkaPublisherContainer.ResponseModel;
using KafkaPublisherContainer.Services.Publisher;
using Microsoft.AspNetCore.Mvc;

namespace KafkaPublisherContainer.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class KafkaController : ControllerBase
    {
        private readonly BaseKafkaService _publisher;
        public KafkaController(BaseKafkaService publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("publish/single")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> FireMessage([FromBody]PublishSingleRequest request)
        {
            var res = await _publisher.PublishToKafka(request.Message, request.Topic, request.Brokers);
            return res == "Ok"
                ? Ok(new StandardResponse("Successfully published message"))
                : StatusCode(500, new StandardResponse(res));
        }

        [HttpPost("publish/multiple")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> FireMultipleMessages([FromBody]PublishMultipleRequest request)
        {
            var res = await _publisher.PublishMultipleMessagesToKafka(request.Message, request.Topic, request.Brokers);
            return res == "Ok"
                ? Ok(new StandardResponse("Successfully published message"))
                : StatusCode(500, new StandardResponse(res));
        }
    }
}