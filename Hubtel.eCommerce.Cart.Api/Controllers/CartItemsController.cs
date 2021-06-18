using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/CartItems")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CartItemsController> _logger;

        public CartItemsController(ApplicationDbContext context, ILogger<CartItemsController> logger)
        {
            _db = context;
            _logger = logger;
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCarts(
            [FromQuery(Name = "phoneNumber")] string phoneNumber = "",
            [FromQuery(Name = "product")] long productId = 0
        )
        {
            var items = await _db.CartItems
                .Where(e => phoneNumber == "" || (e.User.PhoneNumber == phoneNumber))
                .Where(e => productId == 0 || (productId != 0 && e.ProductId == productId))
                .Include(e => e.User)
                .Include(e => e.Product)
                .ToListAsync();

            return Ok(new
            {
                status = HttpStatusCode.OK,
                success = items.Count == 0,
                message = items.Count == 0 ? "No cart item found." : "Found.",
                data = items
            });
        }

        // GET: api/CartItems/user/3
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<CartItemDTO>>> GetUserCart(long id)
        {
            var items = await _db.CartItems
                .Where(e => e.UserId == id)
                .Select(e => new CartItemDTO {
                    Product = e.Product
                })
                .ToListAsync();

            return Ok(new
            {
                status = HttpStatusCode.OK,
                success = items.Count == 0,
                message = items.Count == 0 ? "No cart item found." : "Found.",
                data = items
            });
        }

        // POST: api/CartItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            try
            {
                await ValidateRequestBody(cartItem);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = ex.Message,
                    data = cartItem
                });
            }

            CartItem item = await GetDbItemWithUser(cartItem);

            cartItem.Quantity = cartItem.Quantity != 0 ? cartItem.Quantity : 1;

            if (item != null)
            {
                _db.CartItems.Update(item);
                item.Quantity += cartItem.Quantity;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"Cart item quantity increased for user {cartItem.UserId}");

                return CreatedAtAction(nameof(GetUserCart), new { id = item.UserId }, new
                {
                    status = HttpStatusCode.Created,
                    success = true,
                    message = "Product added to cart successfully",
                    data = item
                });
            }
            else
            {
                _db.Entry(cartItem).State = EntityState.Added;
                _db.CartItems.Add(cartItem);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"New cart item created for user {cartItem.UserId}");

                return CreatedAtAction(nameof(GetUserCart), new { id = cartItem.UserId }, new
                {
                    status = HttpStatusCode.Created,
                    success = true,
                    message = "Product added to cart successfully",
                    data = cartItem
                });
            }
        }

        // DELETE: api/CartItems
        [HttpDelete]
        public async Task<ActionResult<CartItem>> DeleteCartItem(CartItem cartItem)
        {
            CartItem item = await GetDbItem(cartItem);

            cartItem.Quantity = cartItem.Quantity != 0 ? cartItem.Quantity : 1;

            if (item == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    success = false,
                    message = "Cart item not found",
                    data = cartItem
                });
            }

            item.Quantity -= cartItem.Quantity;

            if (item.Quantity >= 1)
            {
                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"Product {cartItem.UserId} removed in user {cartItem.UserId}'s cart");
            }
            else
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"Product {cartItem.UserId} removed in user {cartItem.UserId}'s cart");
            }

            return Ok(new
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Product deleted successfully from cart",
                data = (Object)null
            });
        }

        private bool CartItemExists(long id)
        {
            return _db.CartItems.Any(e => e.CartItemId == id);
        }

        private async Task<CartItem> GetDbItem(CartItem cartItem)
        {
            return await _db.CartItems
               .Where(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId)
               .SingleOrDefaultAsync();
        }

        private async Task<CartItem> GetDbItemWithUser(CartItem cartItem)
        {
            return await _db.CartItems
                .Where(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId)
                .Include(e => e.User)
                .SingleOrDefaultAsync();
        }

        private async Task ValidateRequestBody(CartItem cartItem)
        {
            Product product = await _db.Products.FindAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new ArgumentException("Invalid product");
            }

            User user = await _db.Users.FindAsync(cartItem.UserId);

            if (user == null)
            {
                throw new ArgumentException("Invalid user");
            }
        }
    }
}
