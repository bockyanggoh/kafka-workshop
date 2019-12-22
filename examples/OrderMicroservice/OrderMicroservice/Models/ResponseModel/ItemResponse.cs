namespace OrderMicroservice.ResponseModel
{
    public class ItemResponse<T> where T : class
    {
        public string RequestStatus { get; set; }
        public string TransactionTs { get; set; }
        public T ItemData { get; set; }
    }
}