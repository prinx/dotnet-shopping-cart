namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class Product
    {
        public long ProductId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public bool InStock { get; set; }
    }
}
