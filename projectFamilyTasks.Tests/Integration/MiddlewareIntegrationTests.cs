using System.Net;
using System.Net.Http.Json;

namespace projectFamilyTasks.Tests.Integration;

public class MiddlewareIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public MiddlewareIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [PostgresFact]
    public async Task Request_WithoutToken_Returns401WithAHebrewMessageAndCorrelationId()
    {
        var response = await _factory.CreateClient().GetAsync("/api/house-tasks");

        await response.AssertStatusAsync(HttpStatusCode.Unauthorized);
        var body = await response.ReadJsonAsync();
        Assert.Equal(401, body.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("message").GetString()));
    }

    [PostgresFact]
    public async Task Parent_BeforeChangingThePassword_IsBlockedByTheMiddleware()
    {
        var parent = await _factory.LoginAsync(
            "parent", ApiFactory.InitialParentsPassword, expectMustChangePassword: true);

        var blocked = await parent.GetAsync("/api/children");

        await blocked.AssertStatusAsync(HttpStatusCode.Forbidden);
    }

    [PostgresFact]
    public async Task InvalidBody_Returns400AndTheSameCorrelationIdInTheHeaderAndTheBody()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/auth/register", new
        {
            name = "", userName = "a", password = "1", age = 99
        });

        await response.AssertStatusAsync(HttpStatusCode.BadRequest);
        var body = await response.ReadJsonAsync();
        var headerId = response.Headers.GetValues("X-Correlation-Id").Single();
        Assert.Equal(headerId, body.GetProperty("correlationId").GetString());
    }
}
