using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// -- Builder --
var builder = WebApplication.CreateBuilder(args);

// -- Logging --
// appsettings.* defines logging configuration (12-Factor)
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
		| HttpLoggingFields.ResponseStatusCode;
	o.RequestHeaders.Add("User-Agent");
	o.ResponseHeaders.Add("Content-Type");
	o.CombineLogs = true;
});

// -- Open API --
builder.Services.AddOutputCache(o =>
{
	o.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
});
builder.Services.AddOpenApi("openapi");

// -- Validation --
builder.Services.AddValidation();

// -- Build --
var app = builder.Build();

// -- Exception handling --
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

// -- Logging --
app.UseHttpLogging();

// -- Endpoints --
app.MapGet("/hi", ([FromQuery] [MinLength(3)] string? name) => Hello(name));
app.MapGet("/hi/{name}", Hello);

// -- Open API --
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi().CacheOutput();
}

// -- Run --
await app.RunAsync();
return;

string Hello(string? name) => $"Hello, {name ?? "world"}!";
