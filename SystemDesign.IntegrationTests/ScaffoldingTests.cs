using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SystemDesign.IntegrationTests;

/// <summary>
/// Proves the integration test project can start the real web application and
/// call it over HTTP. This is the difference from a unit test: nothing here is
/// faked — a real server starts, a real request goes out, a real response comes back.
///
/// Leave this in place. If it ever goes red, your wiring is broken, not your logic.
/// </summary>
public class ScaffoldingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ScaffoldingTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task HealthEndpoint_RespondsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
