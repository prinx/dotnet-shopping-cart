using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Object>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    success = false,
                    message = "User not found.",
                    data = (Object)null
                });
            }

            return Ok(new
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "Found.",
                data = user
            });
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(new
                {
                    status = HttpStatusCode.BadRequest,
                    success = false,
                    message = "Invalid User or Id.",
                    data = user
                });
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new
                    {
                        status = HttpStatusCode.NotFound,
                        success = false,
                        message = "User not found.",
                        data = user
                    });
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"User with id {id} updated successfully.");

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.Name.Trim();
            user.PhoneNumber.Trim();

            if (_context.Users.Any(e => user.PhoneNumber == e.PhoneNumber))
            {
                return Conflict(new
                {
                    status = HttpStatusCode.Conflict,
                    success = false,
                    message = "User already exists.",
                    data = user
                });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User with phone number {user.PhoneNumber} created successfully.");

            return CreatedAtAction("GetUser", new { id = user.UserId }, new
            {
                status = HttpStatusCode.Created,
                success = true,
                message = "User created successfully.",
                data = user
            });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new
                {
                    status = HttpStatusCode.NotFound,
                    success = true,
                    message = "User not found.",
                    data = (Object)null
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User with id {id} deleted successfully.");

            return Ok(new
            {
                status = HttpStatusCode.OK,
                success = true,
                message = "User deleted usccessfully.",
                data = (Object)null
            });
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
