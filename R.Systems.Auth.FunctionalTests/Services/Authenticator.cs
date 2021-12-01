using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.WebApi.Features.Authentication;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace R.Systems.Auth.FunctionalTests.Services
{
    public class Authenticator
    {
        public Authenticator(RequestService requestService)
        {
            RequestService = requestService;
        }

        public RequestService RequestService { get; }

        public async Task<AuthenticateResponse> AuthenticateAsync(HttpClient httpClient)
        {
            Users users = new();
            AuthenticateRequest request = new()
            {
                Email = users[0].Email,
                Password = users[0].Password
            };
            (_, AuthenticateResponse? response) = await RequestService.SendPostAsync
                <AuthenticateRequest, AuthenticateResponse>(
                    "/users/authenticate",
                    request,
                    httpClient
                );
            if (response == null)
            {
                throw new Exception("Unexpected error in authentication has occured.");
            }
            return response;
        }
    }
}
