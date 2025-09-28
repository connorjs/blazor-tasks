using System;
using Microsoft.Extensions.Configuration;

namespace Connorjs.BlazorTasks.WebApp.Main;

internal static class ConfigurationExtensions
{
	internal static T GetRequired<T>(this IConfiguration config, string sectionName)
		where T : class
	{
		return config.GetSection(sectionName).Get<T>() ?? throw new InvalidOperationException(
				$"Missing required configuration section: {sectionName}"
			);
	}
}
