using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ApiRest.Application.DTOs;
using FluentAssertions;

namespace ApiRest.Integration.Tests.Products;

public class ProductsEndpointsTests(ApiWebFactory factory) : IClassFixture<ApiWebFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<string> GetAdminTokenAsync()
    {
        // 1. Register
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Admin",
            email = "admin@products.test",
            password = "Admin@123"
        });

        if (!registerResponse.IsSuccessStatusCode)
        {
            var error = await registerResponse.Content.ReadAsStringAsync();
            Console.WriteLine("ERRO REGISTER:");
            Console.WriteLine(error);

            throw new Exception($"Register falhou: {registerResponse.StatusCode}");
        }

        // 2. Promote user
        var db = factory.GetDbContext();
        var user = db.Users.First(u => u.Email == "admin@products.test");

        user.SetAdmin();
        await db.SaveChangesAsync();

        // 3. Login
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@products.test",
            password = "Admin@123"
        });

        if (!loginResponse.IsSuccessStatusCode)
        {
            var error = await loginResponse.Content.ReadAsStringAsync();
            Console.WriteLine("ERRO LOGIN:");
            Console.WriteLine(error);

            throw new Exception($"Login falhou: {loginResponse.StatusCode} - {error}");
        }

        // 4. DEBUG REAL
        var json = await loginResponse.Content.ReadAsStringAsync();
        Console.WriteLine("JSON LOGIN:");
        Console.WriteLine(json);

        // 5. Desserialização segura
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var adminTokens = JsonSerializer.Deserialize<TokenResponse>(json, options);

        if (adminTokens == null || string.IsNullOrEmpty(adminTokens.AccessToken))
            throw new Exception("Token não veio na resposta. Verifique o JSON acima.");

        return adminTokens.AccessToken;
    }

    [Fact]
    public async Task GetAll_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_AsAdmin_ShouldReturn201()
    {
        var token = await GetAdminTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/products", new
        {
            name = "Notebook",
            description = "16GB",
            price = 4999.99m,
            stock = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_WithoutToken_ShouldReturn401()
    {
        var client = factory.CreateClient(); // client sem token

        var response = await client.PostAsJsonAsync("/api/products", new
        {
            name = "X",
            description = "",
            price = 1m,
            stock = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}