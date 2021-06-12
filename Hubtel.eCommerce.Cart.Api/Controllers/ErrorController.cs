using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error-local-development")]
        protected IActionResult ErrorLocalResult(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = feature?.Error;
            var problemDetails = new ProblemDetails
            {
                Status = (int) HttpStatusCode.InternalServerError,
                Instance = feature?.Path,
                Title = $"{ex.GetType().Name}: {ex.Message}",
                Detail = ex.StackTrace,
            };

            return StatusCode(problemDetails.Status.Value, problemDetails);
        }

        [Route("/error")]
        protected IActionResult Error(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = feature?.Error;
            var isDev = webHostEnvironment.EnvironmentName == "Development";
            var problemDetails = new ProblemDetails
            {
                Status = (int) HttpStatusCode.InternalServerError,
                Instance = feature?.Path,
                Title = isDev ? $"{ex.GetType().Name}: {ex.Message}" : "An error occured.",
                Detail = isDev ? ex.StackTrace : null,
            };

            return StatusCode(problemDetails.Status.Value, problemDetails);
        }
    }
}
