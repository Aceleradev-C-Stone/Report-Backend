using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using Xunit;

namespace Report.IntegrationTests
{
    public class IntegrationTestFixture : IDisposable
    {
        public readonly StartupAppFactory Factory;
        public HttpClient Client;

        public IntegrationTestFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions()
            {
                HandleCookies = false,
                BaseAddress = new Uri("http://localhost/"),
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 7
            };

            Factory = new StartupAppFactory();
            Client = Factory.CreateClient(clientOptions);
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }
}