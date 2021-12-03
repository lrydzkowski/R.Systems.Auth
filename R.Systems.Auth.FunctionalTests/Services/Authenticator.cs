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
            UserInfo user = new Users().Data["test@lukaszrydzkowski.pl"];
            AuthenticateRequest request = new()
            {
                Email = user.Email,
                Password = user.Password
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
