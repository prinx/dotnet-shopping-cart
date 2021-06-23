using System.ComponentModel.DataAnnotations;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class Product
    {
        public long ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The price must be a positive integer")]
        public int Price { get; set; }

        [Required]
        public bool InStock { get; set; }
    }
}
