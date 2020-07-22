using System.Linq;
using Report.Core.Models;
using Report.Infra.Services;
using Report.Tests.Helpers;
using Xunit;

namespace Report.Tests.Services
{
    public class TokenServiceTest
    {
        [Fact]
        public void Should_Return_Valid_Token_When_GenerateToken()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var user = fakes.Get<User>().First();

            // Act
            var service = new TokenService(fakeConfig);
            var token = service.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
            Assert.True(service.IsValid(token));
        }

        [Fact]
        public void Should_Return_True_When_IsValid_With_Valid_Token()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            var user = fakes.Get<User>().First();

            // Act
            var service = new TokenService(fakeConfig);
            var token = service.GenerateToken(user);
            var valid = service.IsValid(token);

            // Assert
            Assert.True(valid);
        }

        [Fact]
        public void Should_Return_False_When_IsValid_With_Invalid_Token()
        {
            // Arrange
            var fakes = new Fakes();
            var fakeConfig = fakes.FakeConfiguration().Object;
            // Token with invalid signing key, without id and role claims
            var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIx"   +
                        "MjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iO"   +
                        "nRydWUsImp0aSI6ImVhNzYwZjNhLTNmYTUtNDZlMy1hNWQ2LWJ"  +
                        "iOGNkOGYyMzU4YiIsImlhdCI6MTU5NTM4NjE2MywiZXhwIjoxNT" +
                        "k1Mzg5NzYzfQ.6D4sYkKj_e7y2VVR3uvyFiRVr6Qd8Io42ApXR5jZiuY";

            // Act
            var service = new TokenService(fakeConfig);
            var valid = service.IsValid(token);

            // Assert
            Assert.False(valid);
        }
    }
}