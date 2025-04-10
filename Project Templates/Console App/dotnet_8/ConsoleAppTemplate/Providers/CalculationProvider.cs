using ConsoleAppTemplate.Options;
using Microsoft.Extensions.Options;

namespace ConsoleAppTemplate.Providers
{
	public class CalculationProvider
	{
		private readonly AppSettings appSettings;

		public CalculationProvider(IOptions<AppSettings> appSettings)
		{
			this.appSettings = appSettings.Value;
		}

		public int SumNumbers(int a, int b)
		{
			return a + b;
		}
	}
}