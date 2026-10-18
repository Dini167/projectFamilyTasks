using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using projectFamilyTasks.Application.DTOs.Auth;

namespace projectFamilyTasks.Tests.Integration;

public static class ApiClientExtensions
{
    public static async Task<HttpClient> LoginAsync(
        this ApiFactory factory, string userName, string password, bool expectMustChangePassword = false)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName, password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.Equal(expectMustChangePassword, auth!.MustChangePassword);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        return client;
    }

    public static async Task<HttpClient> LoginParentAfterChangingPasswordAsync(
        this ApiFactory factory, string newPassword)
    {
        var parent = await factory.LoginAsync(
            "parent", ApiFactory.InitialParentsPassword, expectMustChangePassword: true);
        var change = await parent.PutAsJsonAsync("/api/auth/parents-password", new
        {
            currentPassword = ApiFactory.InitialParentsPassword,
            newPassword
        });
        await change.AssertStatusAsync(HttpStatusCode.NoContent);

        return await factory.LoginAsync("parent", newPassword);
    }

    public static async Task RegisterChildAsync(this ApiFactory factory, string userName, string password)
    {
        var register = await factory.CreateClient().PostAsJsonAsync("/api/auth/register", new
        {
            name = "ילד בדיקה", userName, password, age = 9
        });
        await register.AssertStatusAsync(HttpStatusCode.Created);
    }

    public static async Task<JsonElement> ReadJsonAsync(this HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    public static async Task AssertStatusAsync(this HttpResponseMessage response, HttpStatusCode expected)
    {
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {response.StatusCode}: {body}");
    }
}
