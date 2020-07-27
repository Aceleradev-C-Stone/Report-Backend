using Newtonsoft.Json;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.IntegrationTests.Helpers;
using System.Net.Http;
using System.Threading.Tasks;

namespace Report.IntegrationTests.Controllers
{
    public class BaseControllerTest
    {
        protected void SetClientAuthToken(HttpClient client, string token)
        {
            if (client.DefaultRequestHeaders.Contains("Authorization"))
                RemoveClientAuthToken(client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        protected void RemoveClientAuthToken(HttpClient client)
        {
            client.DefaultRequestHeaders.Remove("Authorization");
        }

        protected async Task<LoginUserResponse> LoginAsManager(HttpClient client)
        {
            return await LoginUser(client, "teste.manager@teste.com");
        }

        protected async Task<LoginUserResponse> LoginAsDeveloper(HttpClient client)
        {
            return await LoginUser(client, "teste.developer@teste.com");
        }

        protected async Task<LoginUserResponse> LoginAsDesigner(HttpClient client)
        {
            return await LoginUser(client, "teste.designer@teste.com");
        }

        protected async Task<LoginUserResponse> LoginUser(HttpClient client, string email)
        {
            var data = Fakes.Get<LoginUserRequest>().Find(x => x.Email == email);
            var request = await client.PostAsJsonAsync("api/v1/auth/login", data);
            var response = await request.Content.ReadAsAsync<Response>();
            return JsonConvert.DeserializeObject<LoginUserResponse>(response.Data.ToString());
        }
    }
}
