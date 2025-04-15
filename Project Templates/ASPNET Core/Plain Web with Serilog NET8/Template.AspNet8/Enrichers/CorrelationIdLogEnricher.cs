using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Diagnostics.Enrichment;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Template.AspNet8.Enrichers
{
	public class CorrelationIdLogEnricher : ILogEnricher
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public CorrelationIdLogEnricher(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		}

		public void Enrich(IEnrichmentTagCollector collector)
		{
			var httpContext = httpContextAccessor.HttpContext;
			if (httpContext is not null)
			{
				var httpActivityFeature = httpContext.Features.GetRequiredFeature<IHttpActivityFeature>();
				var activity = httpActivityFeature.Activity;

				var correlationId = activity.GetTagItem("correlationId");
				if (correlationId is not null)
				{
					collector.Add("correlationId", correlationId);
				}
			}
		}
	}

	public class CorrelationIdLogEventEnricher : ILogEventEnricher
	{
		private IHttpContextAccessor httpContextAccessor;
		private readonly IServiceProvider serviceProvider;

		//public CorrelationIdLogEventEnricher()
		//{
		//}

		public CorrelationIdLogEventEnricher(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
		{
			//this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			this.serviceProvider = serviceProvider;
		}

		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			if (httpContextAccessor == null)
			{
				httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
			}

			var httpContext = httpContextAccessor.HttpContext;
			if (httpContext is not null)
			{
				var httpActivityFeature = httpContext.Features.GetRequiredFeature<IHttpActivityFeature>();
				var activity = httpActivityFeature.Activity;

				var correlationId = activity.GetTagItem("CorrelationId");
				if (correlationId is not null)
				{
					logEvent.AddOrUpdateProperty(new LogEventProperty("CorrelationId", new ScalarValue(correlationId)));
				}
			}
		}
	}

	//public static class LoggingExtensions
	//{
	//	public static LoggerConfiguration WithCorrelationId(
	//		this LoggerEnrichmentConfiguration enrich)
	//	{
	//		if (enrich == null)
	//			throw new ArgumentNullException(nameof(enrich));

	//		return enrich.With<CorrelationIdLogEventEnricher>();
	//	}
	//}
}