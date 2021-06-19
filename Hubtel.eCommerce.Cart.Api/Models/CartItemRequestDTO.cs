using System;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartItemRequestDTO
    {
        public long UserId { get; set; }
        public int Quantity { get; set; }
        public long ProductId { get; set; }
    }
}
