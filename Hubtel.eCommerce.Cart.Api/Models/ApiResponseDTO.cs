using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class ApiResponseDTO<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class ApiResponseDTO : ApiResponseDTO<Object>
    {
    }
}
