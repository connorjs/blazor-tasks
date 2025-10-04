using Connorjs.BlazorTasks.WebApp.Main;
using Microsoft.AspNetCore.Builder;

await WebApplication
	.CreateBuilder(args)
	// -- Register services (order can matter) --
	.AddMyLogging() // First (early) so others can use logging
	.AddMyOutputCache()
	.AddMyOpenApi()
	.AddMyEndpoints() // Last (late) because wiring up the endpoints depends on everything else
	// -- End services --
	.Build()
	// -- Middleware pipeline (order matters) --
	.UseMyExceptionHandler() // Run exception handler first (early) to catch all exceptions
	.UseMyLogging()
	.UseMyOutputCache() // Before endpoints so [OutputCache] can attach
	.UseMyEndpoints()
	.UseMyOpenApi() // After endpoints
	// -- End middleware pipeline --
	.RunAsync();
