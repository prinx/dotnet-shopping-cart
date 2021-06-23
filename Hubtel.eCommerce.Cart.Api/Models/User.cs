using System.ComponentModel.DataAnnotations;

namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class User
    {
        public long UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Phone]
        [StringLength(maximumLength:12, MinimumLength = 9, ErrorMessage = "Phone number length must have at least 9 characters and at most 12 characters")]
        public string PhoneNumber { get; set; }
    }
}
