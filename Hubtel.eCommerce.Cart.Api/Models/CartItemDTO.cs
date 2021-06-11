using System;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartItemDTO
    {
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public Product Product { get; set; }
    }
}
