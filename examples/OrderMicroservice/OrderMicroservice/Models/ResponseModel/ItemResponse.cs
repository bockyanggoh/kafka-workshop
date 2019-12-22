namespace OrderMicroservice.Models.ResponseModel
{
    public class ItemResponse<T> where T : class
    {
        public CustomEnum.RequestStatus RequestStatus { get; set; }
        public string TransactionTs { get; set; }
        public T ItemData { get; set; }
        public string ErrorMessage { get; set; }
    }
}