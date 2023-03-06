using System.Security.Cryptography;
using System.Text;

namespace Template0.Extensions;

public static class StringExtensions
{
	public static string ToSHA256(this string value)
	{
		StringBuilder sb = new StringBuilder();

		using (var hash = SHA256.Create())            
		{
			Encoding enc = Encoding.UTF8;
			byte[] result = hash.ComputeHash(enc.GetBytes(value));

			foreach (byte b in result)
				sb.Append(b.ToString("x2"));
		}

		return sb.ToString();
	}
}