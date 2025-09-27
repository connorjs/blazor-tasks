#!/usr/bin/env pwsh

param(
	[Alias('c')]
# The configuration to use. Typically "Debug" or "Release".
	[string]$configuration = "Release"
)

dotnet tool restore
if ($configuration -eq "Release") { dotnet csharpier check . } else { dotnet csharpier format . }
