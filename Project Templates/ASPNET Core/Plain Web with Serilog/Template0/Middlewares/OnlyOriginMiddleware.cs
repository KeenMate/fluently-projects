using Template0.Helpers;

namespace Template0.Middlewares;

public class OnlyOriginMiddleware
{
	private readonly ILogger<OnlyOriginMiddleware> logger;
	private readonly IConfiguration config;
	private readonly RequestDelegate next;

	public OnlyOriginMiddleware(ILogger<OnlyOriginMiddleware> logger, IConfiguration config, RequestDelegate next)
	{
		this.logger = logger;
		this.config = config;
		this.next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var host = context.Request.Host.Host;
		var origin = context.Request.Headers.Origin;

		var allowedOrigins = config.GetAllowedOrigins();

		logger.LogDebug("Current host: {host}, origin: {origin}", host, origin);

		if (string.IsNullOrEmpty(origin) || host.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
		{
			await next(context);
			return;
		}

		if (allowedOrigins.All(x => !x.Equals(origin)))
		{
			logger.LogDebug("Origin not found in allowed origins {allowedOrigins}", string.Join(';', allowedOrigins));

			context.Response.StatusCode = 403;
			await context.Response.CompleteAsync();
		}

		await next(context);
	}
}