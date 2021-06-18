using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public ProductsController(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Object>> GetProduct(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    success = false,
                    message = "Product not found.",
                    data = (Object) null
                });
            }

            return Ok(new
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Found.",
                data = product
            });
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest(new
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Invalid Product or Id.",
                    data = product
                });
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound(new
                    {
                        status = HttpStatusCode.NotFound,
                        success = false,
                        message = "Product not found.",
                        data = product
                    });
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Product with id {id} updated successfully");

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            product.Name.Trim();

            if (_context.Products.Any(e => product.Name == e.Name))
            {
                return Conflict(new
                {
                    status = HttpStatusCode.Conflict,
                    success = false,
                    message = "Product already exists.",
                    data = (Object)null
                });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Product with name '{product.Name}' created successfully.");

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, new
            {
                status = HttpStatusCode.Created,
                success = true,
                message = "Product created successfully.",
                data = product
            });
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Object>> DeleteProduct(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.Created,
                    success = true,
                    message = "Product created successfully.",
                    data = product
                });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Product with id {id} deleted successfully.");

            return Ok(new
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Product deleted successfully.",
                data = (Object)null
            });
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
