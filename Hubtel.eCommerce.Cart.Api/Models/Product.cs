using System.ComponentModel.DataAnnotations;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class Product
    {
        public long ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public bool InStock { get; set; }
    }
}
