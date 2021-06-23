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
    public class ProductsController : CustomBaseController
    {
        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger) : base(context, logger)
        {
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<Product>>>> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Found.",
                    Data = products
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while retrieving products from database: {e}");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO<Product>>> GetProduct(long id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "Product not found.",
                        Data = (Object)null
                    });
                }

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Found.",
                    Data = product
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while retrieving product from database: {e}");
            }
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    return BadRequest(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Invalid product id.",
                        Data = product
                    });
                }

                var productToUpdate = await _context.Products.FindAsync(id);

                if (productToUpdate == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "Product not found.",
                        Data = product
                    });
                }

                _context.Products.Attach(productToUpdate);

                productToUpdate.Name = product.Name;
                productToUpdate.Price = product.Price;
                productToUpdate.InStock = product.InStock;

                await _context.SaveChangesAsync();


                _logger.LogInformation($"Product with id {id} updated successfully");

                return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while updating product: {e}");
            }
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<Product>>> PostProduct(Product product)
        {
            try
            {
                product.Name = product.Name.Trim();

                if (_context.Products.Any(e => product.Name == e.Name))
                {
                    return Conflict(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.Conflict,
                        Success = false,
                        Message = "Product already exists.",
                        Data = (Object)null
                    });
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product with name '{product.Name}' created successfully.");

                return CreatedAtAction("GetProduct", new { id = product.ProductId }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Product created successfully.",
                    Data = product
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while retrieving product from database: {e}");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDTO<Product>>> DeleteProduct(long id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "Product not found.",
                        Data = product
                    });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product with id {id} deleted successfully.");

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Product deleted successfully.",
                    Data = (Object)null
                });

            }
            catch (Exception e)
            {
                return GenericError($"An error happened while deleting product: {e}");
            }
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
