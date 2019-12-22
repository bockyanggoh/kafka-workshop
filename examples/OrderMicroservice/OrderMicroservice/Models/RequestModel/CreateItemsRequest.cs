using System.Collections.Generic;

namespace OrderMicroservice.RequestModel
{
    public class CreateItemsRequest
    {
        public  List<CreateItemRequest> Items { get; set; }
    }
}