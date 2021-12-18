using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using R.Systems.Auth.SharedKernel.Jwt;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.SharedKernel.Middlewares
{
    public class ExceptionMiddleware
    {
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }
        private RequestDelegate Next { get; }

        private ILogger<ExceptionMiddleware> Logger { get; }

        public async Task InvokeAsync(HttpContext httpContext, UserClaimsService userClaimsService)
        {
            string requestUrl = "";
            string requestHttpMethod = "";
            string requestBody = "";
            try
            {
                requestUrl = GetRequestUrl(httpContext);
                requestHttpMethod = GetHttpMethod(httpContext);
                requestBody = await GetRequestBodyAsync(httpContext);
                await Next(httpContext);
            }
            catch (Exception ex)
            {
                ex.Data.Add("UserId", GetUserId(userClaimsService));
                ex.Data.Add("Request URL", requestUrl);
                ex.Data.Add("Request HTTP Method", requestHttpMethod);
                ex.Data.Add("Request Body", requestBody);
                Logger.LogError(ex, message: "An unexpected error occured.");

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private long GetUserId(UserClaimsService userClaimsService)
        {
            return userClaimsService.GetUserId();
        }

        private string GetRequestUrl(HttpContext httpContext)
        {
            return httpContext.Request.Path.Value ?? "";
        }

        private string GetHttpMethod(HttpContext httpContext)
        {
            return httpContext.Request.Method;
        }

        private async Task<string> GetRequestBodyAsync(HttpContext httpContext)
        {
            // 1 MiB
            if (httpContext.Request.ContentLength / 1024 / 1024 > 1)
            {
                return "";
            }
            httpContext.Request.EnableBuffering();
            StreamReader stream = new(httpContext.Request.Body);
            string body = await stream.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;
            body = RemoveSensitiveData(body);
            return body;
        }

        private string RemoveSensitiveData(string body)
        {
            return Regex.Replace(body, "\"password\":\"(.+)\"", "\"password\":\"--removed_by_logger--\"");
        }
    }
}
