namespace OrderMicroservice.ResponseModel
{
    public class CreateItemResponse<T> where T : class
    {
        public string RequestStatus { get; set; }
        public string TransactionTs { get; set; }
        public T ItemData { get; set; }
    }
}