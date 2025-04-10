using CorrelationId.DependencyInjection;

namespace Template0.Extensions
{
	public static class BuilderExtensions
	{
		public static WebApplicationBuilder AddCorrelationId(this WebApplicationBuilder builder)
		{
			builder.Services.AddDefaultCorrelationId(options =>
			{
				options.RequestHeader = "X-CorrelationId";
				options.IncludeInResponse = true;
			});
			return builder;
		}
	}
}