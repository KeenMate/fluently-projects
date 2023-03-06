using System.Security.Cryptography;
using Template0.Extensions;

namespace Template0.Middlewares
{
	public class SecurityHeadersMiddleware
	{
		private readonly RequestDelegate _next;

		public SecurityHeadersMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public Task Invoke(HttpContext httpContext)
		{
			//var nonce = Guid.NewGuid().ToString().ToSHA256();
			//httpContext.Items.Add("nonce", nonce);

			//httpContext.Response.Headers["Content-Security-Policy"] = $"default-src 'self' 'nonce-{nonce}'" +
			//                                                          $";style-src 'self' 'unsafe-inline' fonts.googleapis.com maxcdn.bootstrapcdn.com" +
			//                                                          $";font-src 'self' 'unsafe-inline' fonts.googleapis.com maxcdn.bootstrapcdn.com" +
			//                                                          $";img-src 'self' data: aadcdn.msauthimages.net aadcdn.msftauthimages.net" +
			//                                                          $";connect-src 'self' login.microsoftonline.com graph.microsoft.com aadcdn.msftauthimages.net aadcdn.msauthimages.net";

			httpContext.Response.Headers["X-XSS-Protection"] = "1";
			httpContext.Response.Headers["X-Content-Type-Options"] = "nosniff";
			httpContext.Response.Headers["X-Frame-Options"] = "DENY";

			return _next.Invoke(httpContext);
		}

	}
}