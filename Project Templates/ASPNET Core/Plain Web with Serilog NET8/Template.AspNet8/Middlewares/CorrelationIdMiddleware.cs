using Microsoft.AspNetCore.Http.Features;

namespace Template.AspNet8.Middlewares
{
	public class CorrelationIdMiddleware : IMiddleware
	{
		private readonly IProblemDetailsService _problemDetailsService;

		public CorrelationIdMiddleware(IProblemDetailsService problemDetailsService)
		{
			ArgumentNullException.ThrowIfNull(problemDetailsService);
			_problemDetailsService = problemDetailsService;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var values))
			{
				var correlationId = values.First()!;
				if (correlationId.Length > 128)
				{
					var problemDetails = new HttpValidationProblemDetails
					{
						Detail = "CorrelationId exceeded max length of 128 chars"
					};
					var problemDetailsContext = new ProblemDetailsContext
					{
						HttpContext = context,
						ProblemDetails = problemDetails
					};
					await _problemDetailsService.WriteAsync(problemDetailsContext);
					return;
				}

				context.TraceIdentifier = correlationId;
			}

			var activityFeature = context.Features.GetRequiredFeature<IHttpActivityFeature>();
			var activity = activityFeature.Activity;
			activity.AddTag("CorrelationId", context.TraceIdentifier);
			await next(context);
		}
	}
}