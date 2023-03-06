namespace Template0.Helpers;

public static class ConfigExtensions
{
	public const string AllowedOriginsKey = "ALLOWED_ORIGINS";

	public static List<string> GetAllowedOrigins(this IConfiguration config)
	{
		var value = config.GetValue<string>(AllowedOriginsKey) ?? string.Empty;
		return ParseValue(value);
	}

	public static List<string> GetAllowedOrigins()
	{
		string origins = Environment.GetEnvironmentVariable(AllowedOriginsKey) ?? string.Empty;

		return ParseValue(origins);
	}

	private static List<string> ParseValue(string value)
	{
		return value.Split(";").ToList();
	}
}