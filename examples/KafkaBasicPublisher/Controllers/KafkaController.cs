using System.Threading.Tasks;
using KafkaBasicPublisher.RequestModel;
using KafkaBasicPublisher.Services.Publisher;
using Microsoft.AspNetCore.Mvc;

namespace KafkaBasicPublisher.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class KafkaController : ControllerBase
    {
        private readonly SuperEasyPublisher _publisher;
        public KafkaController(SuperEasyPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("publish/single")]
        public async Task<IActionResult> FireMessage([FromBody]PublishSingleRequest request)
        {
            return await _publisher.Publish(request)
                ? Ok("Successfully published message")
                : StatusCode(500, "Failed to deliver message to Kafka servers. Ensure the servers are available");
        }

        [HttpPost("publish/multiple")]
        public async Task<IActionResult> FireMultipleMessages([FromBody]PublishMultipleRequest request)
        {
            return await _publisher.PublishMultiple(request)
                ? Ok("Successfully published message")
                : StatusCode(500, "Failed to deliver message to Kafka servers. Ensure the servers are available");
        }
    }
}