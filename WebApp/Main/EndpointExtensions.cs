using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class EndpointExtensions
{
	internal static T AddMyEndpoints<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		builder.Services.AddFastEndpoints().AddValidation();
		return builder;
	}

	internal static WebApplication UseMyEndpoints(this WebApplication app)
	{
		app.UseFastEndpoints(static c =>
		{
			c.Errors.UseProblemDetails();
			c.Endpoints.ShortNames = true;
		});
		return app;
	}
}
