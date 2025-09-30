using FastEndpoints.Swagger;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class OpenApiExtensions
{
	[UsedImplicitly]
	private sealed record OpenApiConfig(
		string DocumentName,
		string Title,
		string Description,
		string Version
	);

	internal static T AddMyOpenApi<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		var config = builder.Configuration.GetRequired<OpenApiConfig>("OpenApi");
		builder.Services.SwaggerDocument(o =>
		{
			o.DocumentSettings = s =>
			{
				s.DocumentName = config.DocumentName;
				s.Title = config.Title;
				s.Description = config.Description;
				s.Version = config.Version;
				s.MarkNonNullablePropsAsRequired();
			};
			o.ExcludeNonFastEndpoints = true;
		});
		return builder;
	}

	internal static WebApplication UseMyOpenApi(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			var openApiConfig = app.Configuration.GetRequired<OpenApiConfig>("OpenApi");
			app.UseOpenApi(s => s.Path = "/openapi/{documentName}.json");
			app.MapScalarApiReference(
				"/docs",
				o =>
					o.AddDocument(openApiConfig.DocumentName, "Blazor Tasks BFF | @connorjs")
						.WithDarkMode(false)
			);
		}

		return app;
	}
}
