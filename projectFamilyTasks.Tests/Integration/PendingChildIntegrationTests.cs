using System.Net;
using System.Net.Http.Json;

namespace projectFamilyTasks.Tests.Integration;

public class PendingChildIntegrationTests : IClassFixture<ApiFactory>
{
    private const string ChildPassword = "child12345";

    private readonly ApiFactory _factory;

    public PendingChildIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [PostgresFact]
    public async Task ChildNotYetApproved_CanEnterAndSeeTheProfileButCannotTakeTasks()
    {
        var parent = await _factory.LoginParentAfterChangingPasswordAsync("ParentPass2026");
        await (await parent.PostAsJsonAsync("/api/rounds", new { tasksQuotaPerChild = 1 }))
            .AssertStatusAsync(HttpStatusCode.Created);
        await _factory.RegisterChildAsync("kid2", ChildPassword);

        var child = await _factory.LoginAsync("kid2", ChildPassword);

        var profile = await child.GetAsync("/api/children/me");
        await profile.AssertStatusAsync(HttpStatusCode.OK);
        Assert.False((await profile.ReadJsonAsync()).GetProperty("isApproved").GetBoolean());

        var available = await child.GetAsync("/api/assignments?scope=available&pageSize=50");
        await available.AssertStatusAsync(HttpStatusCode.OK);
        var first = (await available.ReadJsonAsync()).GetProperty("items").EnumerateArray().First();
        var take = await child.PutAsync($"/api/assignments/{first.GetProperty("id").GetInt32()}/assignee", null);
        await take.AssertStatusAsync(HttpStatusCode.Forbidden);
    }
}
