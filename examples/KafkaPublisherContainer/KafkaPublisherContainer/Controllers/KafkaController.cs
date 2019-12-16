using System.Threading.Tasks;
using KafkaPublisherContainer.RequestModel;
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
        public async Task<IActionResult> FireMessage([FromBody]PublishSingleRequest request)
        {
            var res = await _publisher.PublishToKafka(request.Message, request.Topic);
            return res == "Ok"
                ? Ok("Successfully published message")
                : StatusCode(500, res);
        }

        [HttpPost("publish/multiple")]
        public async Task<IActionResult> FireMultipleMessages([FromBody]PublishMultipleRequest request)
        {
            var res = await _publisher.PublishMultipleMessagesToKafka(request.Message, request.Topic);
            return res == "Ok"
                ? Ok("Successfully published message")
                : StatusCode(500, res);
        }
    }
}