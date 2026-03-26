using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ApiRest.Integration.Tests.Auth;

public class AuthEndpointsTests(ApiWebFactory factory)
    : IClassFixture<ApiWebFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_WithValidData_ShouldReturn200WithTokens()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            name     = "Test User",
            email    = "test@integration.com",
            password = "Test@123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<TokenResponse>();

        body!.AccessToken.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturn400()
    {
        var payload = new
        {
            name     = "User",
            email    = "duplicate@test.com",
            password = "Test@123"
        };

        await _client.PostAsJsonAsync("/api/auth/register", payload);
        var second = await _client.PostAsJsonAsync("/api/auth/register", payload);

        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturn400()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "U", email = "wrong@test.com", password = "Correct@1"
        });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "wrong@test.com", password = "Wrong@999"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);