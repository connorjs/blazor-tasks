using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;

namespace Connorjs.BlazorTasks.WebApp.HelloWorld;

internal sealed class SayHello : Endpoint<HelloRequest, HelloResponse>
{
	public override void Configure()
	{
		Get("/hi");
		AllowAnonymous();
	}

	public override async Task HandleAsync(HelloRequest req, CancellationToken ct)
	{
		await Send.OkAsync(new HelloResponse() { Message = $"Hello, {req.Name ?? "world"}!" }, ct);
	}
}

/// <summary>Request for the `SayHello` operation.</summary>
public sealed record HelloRequest
{
	/// <summary>The name to greet.</summary>
	[QueryParam]
	public string? Name { get; init; }
}

/// <summary>Response for the `SayHello` operation.</summary>
public sealed record HelloResponse
{
	/// <summary>The greeting message.</summary>
	public required string Message { get; init; }
}
