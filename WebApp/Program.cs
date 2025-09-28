using System;
using System.ComponentModel.DataAnnotations;
using Connorjs.BlazorTasks.WebApp.Main;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder
	.Services.AddOutputCache(o =>
		o.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)))
	)
	.AddValidation();

var app = builder.AddMyLogging().AddMyOpenApi().Build();

// Middleware chain
// - Run exception handler first (early) in middleware pipeline to catch all exceptions
app.UseMyExceptionHandler().UseMyLogging();

// Endpoints intentionally kept in Program.cs (will switch to Fast Endpoints)
const string tag = "HelloWorld";
app.MapGet("/hi", ([FromQuery] [MinLength(3)] string? name) => Hello(name))
	.Experimental()
	.WithTags(tag);
app.MapGet("/hi/{name}", Hello).Experimental().WithTags(tag);

await app.UseMyOpenApi().RunAsync();
return;

static string Hello(string? name) => $"Hello, {name ?? "world"}!";
