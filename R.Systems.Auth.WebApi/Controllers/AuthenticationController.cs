using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.WebApi.Features.Authentication;
using R.Systems.Auth.WebApi.Features.Tokens;

namespace R.Systems.Auth.WebApi.Controllers;

[ApiController, Route("users")]
public class AuthenticationController : ControllerBase
{
    public AuthenticationController(
        AuthenticateHandler authenticateHandler,
        GenerateNewTokensHandler generateNewTokensHandler)
    {
        AuthenticateHandler = authenticateHandler;
        GenerateNewTokensHandler = generateNewTokensHandler;
    }

    public AuthenticateHandler AuthenticateHandler { get; }
    public GenerateNewTokensHandler GenerateNewTokensHandler { get; }

    [HttpPost, Route("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest request)
    {
        AuthenticateResponse? response = await AuthenticateHandler.HandleAsync(request);
        if (response == null)
        {
            return Unauthorized();
        }
        return Ok(response);
    }

    [HttpPost, Route("generate-new-tokens")]
    public async Task<IActionResult> GenerateNewTokens(GenerateNewTokensRequest request)
    {
        GenerateNewTokensResponse? response = await GenerateNewTokensHandler.HandleAsync(request);
        if (response == null)
        {
            return Unauthorized();
        }
        return Ok(response);
    }
}
