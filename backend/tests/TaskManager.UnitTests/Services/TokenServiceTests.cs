using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TaskManager.Models;
using TaskManager.Services;
using Xunit;

namespace TaskManager.UnitTests.Services;

public class TokenServiceTests
{
    [Fact]
    public void GenerateToken_ReturnsValidJwt_WithExpectedClaims()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience",
            ["Jwt:Secret"] = new string('a', 64),
            ["Jwt:ExpiresMinutes"] = "60"
        }!;

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var service = new TokenService(configuration);
        var user = new User { Id = 123, Email = "user@example.com" };

        var tokenString = service.GenerateToken(user);

        tokenString.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        token.Issuer.Should().Be("TestIssuer");
        token.Audiences.Should().Contain("TestAudience");

        token.Claims.Should().ContainSingle(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
    }
}


