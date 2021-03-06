using System;

namespace KafkaPublisherAvro.ResponseModel
{
    public class StandardResponse
    {
        public StandardResponse(string message)
        {
            Message = message;
            TransactionTs = DateTime.Now.ToLocalTime().ToString();
        }

        public string Message { get; set; }
        public string TransactionTs { get; set; }
    }
}