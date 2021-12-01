﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace R.Systems.Auth.FunctionalTests.Services
{
    public class RequestService
    {
        public async Task<(HttpStatusCode, TResp?)> SendPostAsync<TReq, TResp>(
            string url, TReq request, HttpClient httpClient, string? accessToken = null)
        {
            var requestContent = new StringContent(
                JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"
            );
            HttpRequestMessage requestMessage = new(HttpMethod.Post, url)
            {
                Content = requestContent
            };
            if (accessToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage);
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            TResp? response = JsonSerializer.Deserialize<TResp>(
                responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
            );
            return (httpResponse.StatusCode, response);
        }

        public async Task<(HttpStatusCode, TResp?)> SendGetAsync<TResp>(
            string url, HttpClient httpClient, string? accessToken = null) where TResp : class
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
            if (accessToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage);
            TResp? response = null;
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            try
            {
                response = JsonSerializer.Deserialize<TResp>(
                    responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );
            }
            catch { }
            return (httpResponse.StatusCode, response);
        }
    }
}
