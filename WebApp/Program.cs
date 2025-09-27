using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// -- Builder --
var builder = WebApplication.CreateBuilder(args);

// -- Logging --
// Configure minimal logging (see `appsettings` for configuration).
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
	builder.Logging.AddDebug();
}
builder.Services.AddHttpLogging(o =>
{
	o.LoggingFields =
		HttpLoggingFields.Duration
		| HttpLoggingFields.RequestPath
		| HttpLoggingFields.RequestProtocol
		| HttpLoggingFields.RequestMethod
		| HttpLoggingFields.RequestHeaders
		| HttpLoggingFields.ResponseHeaders
		| HttpLoggingFields.ResponseStatusCode;
	o.RequestHeaders.Add("User-Agent");
	o.ResponseHeaders.Add("Content-Type");
	// Don't enable body logging in prod.
});

// -- Build --
var app = builder.Build();

// --- Logging---
var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.UseHttpLogging();
app.Use(
	async (ctx, next) =>
	{
		var traceId = Activity.Current?.Id ?? ctx.TraceIdentifier;
		using (logger.BeginScope(new Dictionary<string, object?> { ["TraceId"] = traceId }))
		{
			await next();
		}
	}
);

// --- Exception filter ---
app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async ctx =>
	{
		var ex = ctx
			.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
			?.Error;
		logger.LogError(ex, "Unhandled exception");
		ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
		ctx.Response.ContentType = "application/problem+json";
		await Results.Problem(statusCode: 500, title: "Unexpected error").ExecuteAsync(ctx);
	});
});

// -- Endpoints --
app.MapGet("/", () => "Hello, world!");

// -- Run --
await app.RunAsync();
