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
    public class UsersController : CustomBaseController
    {
        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger) : base(context, logger)
        {
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var items = await _context.Users.ToListAsync();

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = items.Count != 0,
                    Message = items.Count == 0 ? "No user found." : "Found.",
                    Data = items
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while retrieving users from database: {e}");
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Object>> GetUser(long id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "User not found.",
                        Data = (Object)null
                    });
                }

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Found.",
                    Data = user
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while retrieving user from database: {e}");
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Success = false,
                    Message = "Invalid User or Id.",
                    Data = user
                });
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException DbUpdateException)
            {
                try
                {
                    if (!UserExists(id))
                    {
                        return NotFound(new ApiResponseDTO
                        {
                            Status = (int)HttpStatusCode.NotFound,
                            Success = false,
                            Message = "User not found.",
                            Data = user
                        });
                    }
                    else
                    {
                        return GenericError($"An error happened while updating user: {DbUpdateException}");
                    }
                }
                catch (Exception e)
                {
                    return GenericError($"An error happened while updating user: {e}");
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
            try
            {
                user.Name = user.Name.Trim();
                user.PhoneNumber = user.PhoneNumber.Trim();

                if (_context.Users.Any(e => user.PhoneNumber == e.PhoneNumber))
                {
                    return Conflict(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.Conflict,
                        Success = false,
                        Message = "User already exists.",
                        Data = user
                    });
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User with phone number {user.PhoneNumber} created successfully.");


                return CreatedAtAction("GetUser", new { id = user.UserId }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "User created successfully.",
                    Data = user
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while creating user: {e}");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(long id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "User not found.",
                        Data = (Object)null
                    });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User with id {id} deleted successfully.");

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "User deleted usccessfully.",
                    Data = (Object)null
                });
            }
            catch (Exception e)
            {
                return GenericError($"An error happened while deleting user with id {id}: {e}");
            }
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
