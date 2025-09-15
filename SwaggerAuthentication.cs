using System.Collections.Concurrent;

namespace your_name_space
{
    public class SwaggerAuthentication
    {
        private readonly RequestDelegate _next;

        private static readonly ConcurrentDictionary<string, (int Attempts, DateTime LastAttempt)> FailedLogins
            = new ConcurrentDictionary<string, (int, DateTime)>();

        private const int MaxAttempts = 5;
        private static readonly TimeSpan LockoutTime = TimeSpan.FromMinutes(5);

        public SwaggerAuthentication(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                if (FailedLogins.TryGetValue(remoteIp, out var info))
                {
                    if (info.Attempts >= MaxAttempts && DateTime.UtcNow - info.LastAttempt < LockoutTime)
                    {
                        context.Response.StatusCode = 429;
                        await context.Response.WriteAsync("Too many failed attempts. Try again later.");
                        return;
                    }
                }

                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    var encoded = authHeader.Substring("Basic ".Length).Trim();
                    var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                    var parts = decoded.Split(':', 2);

                    var username = parts[0];
                    var password = parts[1];

                    if (username == "admin" && password == "admin")
                    {
                        FailedLogins.TryRemove(remoteIp, out _);
                        await _next(context);
                        return;
                    }
                }

                FailedLogins.AddOrUpdate(remoteIp,
                    _ => (1, DateTime.UtcNow),
                    (_, old) => (old.Attempts + 1, DateTime.UtcNow));

                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authentication required.");
                return;
            }

            await _next(context);
        }
    }

    public static class SwaggerAuthenticationExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthentication>();
        }
    }
}
