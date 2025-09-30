using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class OutputCacheExtensions
{
	[UsedImplicitly]
	private sealed record OutputCacheConfig(
		bool Enabled,
		OutputCachePolicyConfig? BasePolicy = null,
		IDictionary<string, OutputCachePolicyConfig>? Policies = null
	);

	[UsedImplicitly]
	private sealed record OutputCachePolicyConfig(int ExpireSeconds);

	internal static T AddMyOutputCache<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		var config = builder.Configuration.GetRequired<OutputCacheConfig>("OutputCache");
		if (config.Enabled)
		{
			builder.Services.AddOutputCache(o =>
			{
				if (config.BasePolicy is not null)
				{
					o.AddBasePolicy(FromConfig(config.BasePolicy));
				}
				if (config.Policies is not null)
				{
					foreach (var policy in config.Policies)
					{
						o.AddPolicy(policy.Key, FromConfig(policy.Value));
					}
				}
			});
		}
		return builder;
	}

	internal static WebApplication UseMyOutputCache(this WebApplication app)
	{
		var config = app.Configuration.GetRequired<OutputCacheConfig>("OutputCache");
		if (config.Enabled)
		{
			app.UseOutputCache();
		}
		return app;
	}

	private static Action<OutputCachePolicyBuilder> FromConfig(OutputCachePolicyConfig config) =>
		(policy) => policy.Expire(TimeSpan.FromSeconds(config.ExpireSeconds));
}
