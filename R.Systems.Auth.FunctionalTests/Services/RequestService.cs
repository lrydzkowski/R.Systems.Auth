using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace R.Systems.Auth.FunctionalTests.Services
{
    public class RequestService
    {
        public async Task<(HttpStatusCode, TResp?)> SendPostAsync<TReq, TResp>(
            string url, TReq request, HttpClient httpClient)
        {
            var requestContent = new StringContent(
                JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"
            );
            HttpResponseMessage httpResponse = await httpClient.PostAsync(url, requestContent);
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            TResp? response = JsonSerializer.Deserialize<TResp>(
                responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
            );
            return (httpResponse.StatusCode, response);
        }
    }
}
