using Microsoft.Extensions.Configuration;
using System.IO;

namespace MyNewNamespace
{
	public class ConnectionStrings
	{
		private static IConfiguration _configuration;

		public ConnectionStrings(IConfiguration configuration)
		{
			_configuration = configuration;

			Development = _configuration.GetSection("App:ConnectionStrings:Development").Value;
			Production = _configuration.GetSection("App:ConnectionStrings:Production").Value;
		}

		public string Development { get; }
		public string Production { get; }
	}
	public class Profile
	{
		private static IConfiguration _configuration;

		public Profile(IConfiguration configuration)
		{
			_configuration = configuration;

			Machine = _configuration.GetSection("App:Profile:Machine").Value;
			Server = new Server(_configuration);
		}

		public string Machine { get; }
		public Server Server { get; }
	}
	public class Window
	{
		private static IConfiguration _configuration;

		public Window(IConfiguration configuration)
		{
			_configuration = configuration;

			Height = byte.Parse(_configuration.GetSection("App:Window:Height").Value);
			SomeBool = bool.Parse(_configuration.GetSection("App:Window:SomeBool").Value);
			Width = byte.Parse(_configuration.GetSection("App:Window:Width").Value);
		}

		public byte Height { get; }
		public bool SomeBool { get; }
		public byte Width { get; }
	}
	public class Server
	{
		private static IConfiguration _configuration;

		public Server(IConfiguration configuration)
		{
			_configuration = configuration;

			Domain = _configuration.GetSection("App:Profile:Server:Domain").Value;
			IP = _configuration.GetSection("App:Profile:Server:IP").Value;
			Kind = _configuration.GetSection("App:Profile:Server:Kind").Value;
			OS = _configuration.GetSection("App:Profile:Server:OS").Value;
			Version = float.Parse(_configuration.GetSection("App:Profile:Server:Version").Value.Replace(".", ","));
		}

		public string Domain { get; }
		public string IP { get; }
		public string Kind { get; }
		public string OS { get; }
		public float Version { get; }
	}

	public class AppConfig
	{
		private static IConfiguration _configuration;

		public AppConfig(string jsonPath)
		{
			var builder = new ConfigurationBuilder()
										.SetBasePath(Directory.GetCurrentDirectory())
										.AddJsonFile(jsonPath);
			_configuration = builder.Build();

			ConnectionStrings = new ConnectionStrings(_configuration);
			Profile = new Profile(_configuration);
			Window = new Window(_configuration);
		}

		public ConnectionStrings ConnectionStrings { get; }
		public Profile Profile { get; }
		public Window Window { get; }
	}
}