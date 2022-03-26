using Microsoft.AspNetCore.Mvc;

namespace StocksApi.Web.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult InternalServerErrorResult()
        {
            var details = new ProblemDetails()
            {
                Status = 500,
                Title = "Internal Server Error",
                Detail = "Something went wrong. Please try again."
            };
            return StatusCode(500, details);
        }

        protected IActionResult NotFoundResult()
        {
            var details = new ProblemDetails()
            {
                Status = 404,
                Title = "Not Found",
                Detail = "The requested resource was not found."
            };
            return StatusCode(404, details);
        }
    }
}
