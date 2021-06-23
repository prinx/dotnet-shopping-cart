using System.Net;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger _logger;

        public CustomBaseController(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        protected ObjectResult GenericError(string logMessage, string message = "An error happened.")
        {
             _logger.LogError(logMessage);

            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Success = false,
                Message = message,
                Data = null
            });
        }
    }
}