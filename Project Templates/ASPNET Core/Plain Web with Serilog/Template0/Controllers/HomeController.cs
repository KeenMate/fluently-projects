using Template0.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime;

namespace Template0.Controllers
{
	[Route("/")]
	public class HomeController : ControllerBase
	{
		private readonly ILogger<HomeController> logger;
		private readonly IConfiguration config;
		private readonly IWebHostEnvironment environment;

		public HomeController(ILogger<HomeController> logger, IConfiguration config, IWebHostEnvironment environment)
		{
			this.logger = logger;
			this.config = config;
			this.environment = environment;

			logger.LogDebug($"{nameof(HomeController)} initialized");

		}

		[HttpGet]
		public IActionResult Index()
		{
			logger.LogInformation("Getting admin page");

			//var nonce = HttpContext.Items["nonce"]?.ToString();

			//var indexContent = System.IO.File.ReadAllText(Path.Combine(environment.WebRootPath, "index.html"));
			//indexContent = indexContent.Replace("<script ", $"<script nonce=\"{nonce}\" ");
			//return Content(indexContent, MediaTypeNames.Text.Html);

			return Ok("Hello World! The time is " + DateTime.Now.ToString("HH:mm:ss"));
		}

		//[HttpGet("env")]
		//public IActionResult OnGetValues()
		//{
		//	return Ok(new { origins = this.config.GetAllowedOrigins(), domains = this.config.GetAllowedUploadDomains() });
		//}

		//[HttpGet("headers")]
		//public IActionResult OnGetHeaders()
		//{
		//	return Ok(new { headers = Request.Headers });
		//}
	}
}
