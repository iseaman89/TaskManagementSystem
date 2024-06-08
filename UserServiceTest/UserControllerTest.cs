using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using UserService;
using UserService.Models.Users;
using Xunit;

namespace UserServiceTest;

public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UserControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_ReturnsUser()
    {
        // Arrange
        var secretKey = "super_secret_key_123!";
        var issuer = "testIssuer";
        var audience = "testAudience";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "testuser@tms.com"),
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var token = JwtTokenUtil.GenerateJwtToken(secretKey, issuer, audience, claims);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/user/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<UserDto>(responseString);

        Assert.Equal("testuser@tms.com", user.FirstName);
    }

    [Fact]
    public async Task CreateUser_CreatesUser()
    {
        // Arrange
        var userDto = new UserDto
        {
            FirstName = "Albert",
            LastName = "Bach",
            Email = "newuser@example.com",
            Password = "Password123!"
        };
        var content = new StringContent(JsonConvert.SerializeObject(userDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/user", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var createdUser = JsonConvert.DeserializeObject<UserDto>(responseString);

        Assert.Equal("newuser@example.com", createdUser.Email);
    }
}
