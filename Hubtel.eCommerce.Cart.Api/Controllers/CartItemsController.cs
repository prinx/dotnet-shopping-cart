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
using Microsoft.AspNetCore.Hosting;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/CartItems")]
    [ApiController]
    public class CartItemsController : CustomBaseController
    {
        public CartItemsController(ApplicationDbContext context, ILogger<CartItemsController> logger) : base(context, logger)
        {
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<CartItem>>>> GetCarts(
            [FromQuery(Name = "phoneNumber")] string phoneNumber = "",
            [FromQuery(Name = "product")] long productId = 0
        )
        {
            try
            {
                var items = await _context.CartItems
                    .Where(e => phoneNumber == "" || (e.User.PhoneNumber == phoneNumber))
                    .Where(e => productId == 0 || (productId != 0 && e.ProductId == productId))
                    .Include(e => e.User)
                    .Include(e => e.Product)
                    .ToListAsync();

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = items.Count != 0,
                    Message = items.Count == 0 ? "No cart item found." : "Found.",
                    Data = items
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while retrieving cart items from database: {e}");
            }
        }

        // GET: api/CartItems/user/3
        [HttpGet("user/{id}")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<CartItemDTO>>>> GetUserCart(long id)
        {
            try
            {
                var items = await _context.CartItems
                    .Where(e => e.UserId == id)
                    .Select(e => new CartItemDTO {
                        CartItemId = e.CartItemId,
                        Quantity = e.Quantity,
                        CreatedAt = e.CreatedAt,
                        Product = e.Product
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = items.Count == 0 ? "No cart item found." : "Found.",
                    Data = items
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while retrieving cart of user with id {id} from database: {e}");
            }
        }

        // POST: api/CartItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<CartItem>>> PostCartItem([FromBody] CartItemRequestDTO cartItem)
        {
            try
            {
                await ValidateRequestBody(cartItem);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Success = false,
                    Message = ex.Message,
                    Data = cartItem
                });
            }

            try
            {
                CartItem item = await GetDbItemWithUser(cartItem);

                if (item != null)
                {
                    _context.CartItems.Update(item);
                    item.Quantity += cartItem.Quantity;
                    await _context.SaveChangesAsync();
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
                    _context.CartItems.Add(new CartItem
                    {
                        UserId = cartItem.UserId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity
                    });
                    await _context.SaveChangesAsync();
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
            catch (Exception e)
            {
                return GenericError($"An error happened while creating a cart item: {e}");
            }
        }

        // DELETE: api/CartItems
        [HttpDelete]
        public async Task<ActionResult<ApiResponseDTO>> DeleteCartItem([FromBody] CartItemRequestDTO cartItem)
        {
            try
            {
                CartItem item = await GetDbItem(cartItem);

                if (item == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "Cart item not found",
                        Data = cartItem
                    });
                }

                item.Quantity -= cartItem.Quantity;

                if (item.Quantity >= 1)
                {
                    _context.CartItems.Attach(item);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Product {cartItem.UserId} removed in user {cartItem.UserId}'s cart");
                }
                else
                {
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Product {cartItem.UserId} removed in user {cartItem.UserId}'s cart");
                }

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Product deleted successfully from cart",
                    Data = (Object)null
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while deleting a cart item: {e}");
            }
        }

        private bool CartItemExists(long id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }

        private async Task<CartItem> GetDbItem(CartItemRequestDTO cartItem)
        {
            return await _context.CartItems
               .Where(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId)
               .FirstOrDefaultAsync();
        }

        private async Task<CartItem> GetDbItemWithUser(CartItemRequestDTO cartItem)
        {
            return await _context.CartItems
                .Where(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId)
                .Include(e => e.User)
                .FirstOrDefaultAsync();
        }

        private async Task ValidateRequestBody(CartItemRequestDTO cartItem)
        {
            if (cartItem.Quantity <= 0)
            {
                throw new ArgumentException("Invalid product quantity.");
            }

            Product product = await _context.Products.FindAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new ArgumentException("Invalid product.");
            }

            User user = await _context.Users.FindAsync(cartItem.UserId);

            if (user == null)
            {
                throw new ArgumentException("Invalid user.");
            }
        }
    }
}
