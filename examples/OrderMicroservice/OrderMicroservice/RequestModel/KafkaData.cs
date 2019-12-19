namespace OrderMicroservice.RequestModel
{
    public class KafkaData<T> where T : class
    {
        public string Topic { get; set; }
        public T Data { get; set; }
    }
}