using System.Net;
using System.Net.Http.Json;

namespace projectFamilyTasks.Tests.Integration;

public class FullFlowIntegrationTests : IClassFixture<ApiFactory>
{
    private const string ChildPassword = "child12345";

    private readonly ApiFactory _factory;

    public FullFlowIntegrationTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [PostgresFact]
    public async Task ParentAndChild_RunAFullRound_ChildGetsThePoints()
    {
        var parent = await _factory.LoginParentAfterChangingPasswordAsync("ParentPass2026");
        await RegisterAndApproveChildAsync(parent);

        var round = await parent.PostAsJsonAsync("/api/rounds", new { tasksQuotaPerChild = 1 });
        await round.AssertStatusAsync(HttpStatusCode.Created);
        var roundId = (await round.ReadJsonAsync()).GetProperty("id").GetInt32();

        var child = await _factory.LoginAsync("kid1", ChildPassword);
        await TakeFirstAvailableTaskAsync(child);

        await (await parent.PatchAsJsonAsync($"/api/rounds/{roundId}", new { status = 1 }))
            .AssertStatusAsync(HttpStatusCode.NoContent);

        var pointsEarned = await CompleteMyTaskAsync(child);
        var profile = await child.GetAsync("/api/children/me");
        await profile.AssertStatusAsync(HttpStatusCode.OK);
        Assert.Equal(pointsEarned, (await profile.ReadJsonAsync()).GetProperty("pointsBalance").GetInt32());
    }

    private async Task RegisterAndApproveChildAsync(HttpClient parent)
    {
        await _factory.RegisterChildAsync("kid1", ChildPassword);

        var children = await parent.GetAsync("/api/children");
        await children.AssertStatusAsync(HttpStatusCode.OK);
        var child = (await children.ReadJsonAsync()).GetProperty("items").EnumerateArray()
            .Single(c => c.GetProperty("userName").GetString() == "kid1");
        Assert.False(child.GetProperty("isApproved").GetBoolean());

        var childId = child.GetProperty("id").GetInt32();
        await (await parent.PutAsync($"/api/children/{childId}/approval", null))
            .AssertStatusAsync(HttpStatusCode.NoContent);
    }

    private static async Task TakeFirstAvailableTaskAsync(HttpClient child)
    {
        var available = await child.GetAsync("/api/assignments?scope=available&pageSize=50");
        await available.AssertStatusAsync(HttpStatusCode.OK);
        var first = (await available.ReadJsonAsync()).GetProperty("items").EnumerateArray().First();

        await (await child.PutAsync($"/api/assignments/{first.GetProperty("id").GetInt32()}/assignee", null))
            .AssertStatusAsync(HttpStatusCode.NoContent);
    }

    private static async Task<int> CompleteMyTaskAsync(HttpClient child)
    {
        var mine = await child.GetAsync("/api/assignments?scope=mine&pageSize=50");
        await mine.AssertStatusAsync(HttpStatusCode.OK);
        var task = (await mine.ReadJsonAsync()).GetProperty("items").EnumerateArray().Single();
        Assert.Equal(0, task.GetProperty("completionsInPeriod").GetInt32());
        Assert.True(task.GetProperty("periodLimit").GetInt32() >= 1);

        var done = await child.PostAsync($"/api/assignments/{task.GetProperty("id").GetInt32()}/completions", null);
        await done.AssertStatusAsync(HttpStatusCode.Created);
        var after = await child.GetAsync("/api/assignments?scope=mine&pageSize=50");
        var updated = (await after.ReadJsonAsync()).GetProperty("items").EnumerateArray().Single();
        Assert.Equal(1, updated.GetProperty("completionsInPeriod").GetInt32());
        var pointsEarned = (await done.ReadJsonAsync()).GetProperty("pointsEarned").GetInt32();
        Assert.True(pointsEarned > 0);
        return pointsEarned;
    }
}
