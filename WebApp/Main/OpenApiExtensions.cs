using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class OpenApiExtensions
{
	[UsedImplicitly]
	private sealed record OpenApiConfig(string DocumentName, OpenApiInfo Info);

	internal static T AddMyOpenApi<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		var config = builder.Configuration.GetRequired<OpenApiConfig>("OpenApi");
		builder.Services.AddOpenApi(
			config.DocumentName,
			o =>
				o.AddDocumentTransformer(
						(document, _, _) =>
						{
							document.Info = config.Info;
							return Task.CompletedTask;
						}
					)
					.AddScalarTransformers()
		);
		return builder;
	}

	internal static WebApplication UseMyOpenApi(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			var openApiConfig = app.Configuration.GetRequired<OpenApiConfig>("OpenApi");
			app.MapOpenApi().CacheOutput();
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
