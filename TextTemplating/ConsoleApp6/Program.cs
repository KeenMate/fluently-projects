using System;

namespace MyNewNamespace
{
	class Program
	{
		static void Main(string[] args)
		{
			AppConfig app = new AppConfig("appsettings.json");
			Console.WriteLine(app.ConnectionStrings.Development);
			Console.WriteLine(app.ConnectionStrings.Production);
			Console.WriteLine(app.Profile.Machine);
			Console.WriteLine(app.Profile.Server.Domain);
			Console.WriteLine(app.Profile.Server.OS);
			Console.WriteLine(app.Profile.Server.Kind);
			Console.WriteLine(app.Profile.Server.Version);
			Console.WriteLine(app.Profile.Server.IP);
			Console.WriteLine(app.Window.Height);
			Console.WriteLine(app.Window.Width);
			Console.ReadKey();
		}
	}
}
