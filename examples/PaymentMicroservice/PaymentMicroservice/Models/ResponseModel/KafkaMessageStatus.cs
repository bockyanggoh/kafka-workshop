﻿namespace OrderMicroservice.Models.ResponseModel
{
    public class KafkaMessageStatus<T> where T : class
    {
        public string CorrelationId { get; set; }
        public bool Success { get; set; }
        public string ErrorInfo { get; set; }
        public T Data { get; set; }
    }
}