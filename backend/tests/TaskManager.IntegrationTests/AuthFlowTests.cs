using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using TaskManager.Dto;
using Xunit;

namespace TaskManager.IntegrationTests;

public class AuthFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterThenLoginReturnsToken()
    {
        var client = _factory.CreateClient();

        var registerPayload = new { email = "alice@example.com", password = "Passw0rd!" };
        var registerResp = await client.PostAsJsonAsync("/auth/register", registerPayload);
        registerResp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var registerData = await registerResp.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>()!;
        registerData!.Success.Should().BeTrue();
        registerData.Data!.Token.Should().NotBeNullOrWhiteSpace();

        var loginPayload = new { email = "alice@example.com", password = "Passw0rd!" };
        var loginResp = await client.PostAsJsonAsync("/auth/login", loginPayload);
        loginResp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var loginData = await loginResp.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>()!;
        loginData!.Success.Should().BeTrue();
        loginData.Data!.Token.Should().NotBeNullOrWhiteSpace();
    }

    private record AuthResponse(string Token);
}


