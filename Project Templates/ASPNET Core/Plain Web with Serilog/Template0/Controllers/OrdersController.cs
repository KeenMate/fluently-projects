using System.IdentityModel.Tokens.Jwt;
using Template0.Models;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;

namespace Template0.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{

		private readonly ILogger<OrdersController> logger;
		private readonly IConfiguration config;

		public OrdersController(ILogger<OrdersController> logger, IConfiguration config)
		{
			this.logger = logger;
			this.config = config;

			logger.LogDebug($"{nameof(OrdersController)} initialized");
		}


		[HttpPost]
		public async Task<IActionResult> OnPostAsync([FromQuery] OrderModel request)
		{
			logger.LogInformation("POST of {productName}", request.ProductName);

			try
			{
				if (request.Amount <= 0)
				{
					throw new ArgumentException("Amount must be greater than 0");
				}

				if (request.ProductName.Equals("rotten egg", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new ArgumentException("Product name cannot be rotten egg");
				}

				return Ok(request);

			}
			catch (Exception e)
			{
				logger.LogError(e, "Post error with {productName}", request.ProductName);

				return StatusCode(400, e.Message);
			}
		}
	}
}
