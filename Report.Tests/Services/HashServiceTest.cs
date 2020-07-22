using System.Linq;
using Report.Core.Dto.Requests;
using Report.Core.Models;
using Report.Infra.Services;
using Report.Tests.Comparers;
using Report.Tests.Helpers;
using Xunit;

namespace Report.Tests.Services
{
    public class HashServiceTest
    {
        [Fact]
        public void Should_Return_Expected_Hash_When_GenerateSaltedHash()
        {
            // Arrange
            var fakes = new Fakes();
            var password = "SomePassword";

            // Act
            var service = new HashService();
            var actual = service.GenerateSaltedHash(password);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<SaltedHash>(actual);
            Assert.True(service.AreEqual(password, actual));
        }
    }
}