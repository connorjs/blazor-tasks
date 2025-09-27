using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// -- Builder --
var builder = WebApplication.CreateBuilder(args);

// -- Logging --
// Console picks up JSON + options from appsettings.* (12-Factor)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.Configure(o =>
{
	o.ActivityTrackingOptions =
		ActivityTrackingOptions.SpanId
		| ActivityTrackingOptions.TraceId
		| ActivityTrackingOptions.ParentId
		| ActivityTrackingOptions.Tags
		| ActivityTrackingOptions.Baggage;
});
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
});

// -- Build --
var app = builder.Build();

// --- Exception handling ---
// Run first (early) in middleware pipeline to catch all exceptions
app.UseExceptionHandler(errorApp =>
{
	errorApp.Run(async ctx =>
	{
		var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
		logger.LogError(ctx.Features.Get<IExceptionHandlerFeature>()?.Error, "Unhandled exception");

		ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
		ctx.Response.ContentType = "application/problem+json";
		await Results.Problem(statusCode: 500, title: "Unexpected error").ExecuteAsync(ctx);
	});
});

// --- Logging ---
app.UseHttpLogging();

// -- Endpoints --
app.MapGet("/", () => "Hello, world!");

// -- Run --
await app.RunAsync();
