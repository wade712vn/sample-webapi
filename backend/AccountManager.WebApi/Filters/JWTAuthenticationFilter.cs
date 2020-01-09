using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using AccountManager.Application.Auth;
using AccountManager.Infrastructure.Auth;
using Microsoft.IdentityModel.Tokens;

namespace AccountManager.WebApi.Filters
{
    public class JwtAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        public JwtAuthenticationFilter()
        {
        }

        public bool AllowMultiple { get; }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            

            var dependencyScope = context.Request.GetDependencyScope();
            var tokenHandler = (IJwtTokenHandler)dependencyScope.GetService(typeof(IJwtTokenHandler));

            AuthenticationHeaderValue authentication = null;
            authentication = context.Request.Headers.Authorization;

            if (authentication == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", context.Request);
                return;
            }

            var jwtIssuerOptions = (JwtIssuerOptions) dependencyScope.GetService(typeof(JwtIssuerOptions));
            var principal = tokenHandler.ValidateToken(authentication.Parameter, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuerOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtIssuerOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtIssuerOptions.SigningCredentials.Key,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            });

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", context.Request);
                return;
            }

            context.Principal = principal;
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string content, HttpRequestMessage request)
        {
            Content = content;
            Request = request;
        }

        public string Content { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.ReasonPhrase = "Unauthorized";
            response.Content = new StringContent(Content);
            return response;
        }
    }
}