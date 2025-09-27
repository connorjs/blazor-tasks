using System;
using Microsoft.Extensions.Configuration;

namespace Connorjs.BlazorTasks.WebApp.Configuration;

internal static class ConfigurationExtensions
{
	public static T GetRequired<T>(this IConfiguration config, string sectionName)
		where T : class =>
		config.GetSection(sectionName).Get<T>() ?? throw new InvalidOperationException(
			$"Missing required configuration section: {sectionName}"
		);
}
