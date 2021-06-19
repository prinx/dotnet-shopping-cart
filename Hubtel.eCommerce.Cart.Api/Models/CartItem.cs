using System;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartItem
    {
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }

        public CartItem()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
