using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice.RequestModel
{
    public class OrderUserRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public List<string> OrderIds { get; set; }
    }
}