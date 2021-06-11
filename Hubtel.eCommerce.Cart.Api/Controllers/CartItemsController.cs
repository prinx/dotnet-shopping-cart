using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/CartItems")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CartItemsController(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCarts(
            [FromQuery(Name = "phoneNumber")] string phoneNumber = "",
            [FromQuery(Name = "product")] long productId = 0
        )
        {
            return await _db.CartItems
                .Where(e => e.User.PhoneNumber == phoneNumber)
                .Where(e => productId == 0 || (productId != 0 && e.ProductId == productId))
                .Include(e => e.User)
                .Include(e => e.Product)
                .ToListAsync();
        }

        // GET: api/CartItems/user/3
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetUserCart(long id)
        {
            return await _db.CartItems
                .Where(e => e.UserId == id)
                .Include(e => e.User)
                .Include(e => e.Product)
                .ToListAsync();
        }

        // GET: api/CartItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(long id)
        {
            var cartItem = await _db.CartItems
                .Where(e => e.CartItemId == id)
                .Include(e => e.User)
                .Include(e => e.Product)
                .FirstOrDefaultAsync();

            if (cartItem == null)
            {
                return NotFound();
            }

            return cartItem;
        }

        // PUT: api/CartItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(long id, CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return BadRequest();
            }

            _db.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CartItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            CartItem item = await _db.CartItems
                .Where(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId)
                .Include(e => e.User)
                .SingleOrDefaultAsync();

            cartItem.Quantity = cartItem.Quantity != 0 ? cartItem.Quantity : 1;

            if (item != null)
            {
                _db.CartItems.Update(item);
                item.Quantity += cartItem.Quantity;
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserCart), new { id = item.UserId }, item);
            }
            else
            {
                _db.Entry(cartItem).State = EntityState.Added;
                _db.CartItems.Add(cartItem);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserCart), new { id = cartItem.UserId }, cartItem);
            }

        }

        // DELETE: api/CartItems
        [HttpDelete]
        public async Task<ActionResult<CartItem>> DeleteCartItem(CartItem cartItem)
        {
            CartItem item = await _db.CartItems
               .Where(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId)
               .SingleOrDefaultAsync();

            cartItem.Quantity = cartItem.Quantity != 0 ? cartItem.Quantity : 1;

            if (item == null)
            {
                return NotFound();
            }

            item.Quantity -= cartItem.Quantity;

            if (item.Quantity >= 1)
            {
                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            else
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }

            return item;
        }

        // DELETE: api/CartItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CartItem>> DeleteProductFromCartItem(long id)
        {
            var cartItem = await _db.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _db.CartItems.Remove(cartItem);
            await _db.SaveChangesAsync();

            return cartItem;
        }

        private bool CartItemExists(long id)
        {
            return _db.CartItems.Any(e => e.CartItemId == id);
        }

    }
}
