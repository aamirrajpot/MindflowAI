using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindflowAI.Services.Dtos.Google;
using OpenIddict.Abstractions;
using System.Security.Claims;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MindflowAI.Services.Google
{
    public class GoogleAuthService : ApplicationService, IGoogleAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictTokenManager _tokenManager;
        private readonly ICurrentTenant _currentTenant;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;
        private readonly IdentityUserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly string _googleClientId;


        public GoogleAuthService(IHttpClientFactory httpClientFactory, IOpenIddictApplicationManager applicationManager, IOpenIddictTokenManager tokenManager, ICurrentTenant currentTenant, IOpenIddictAuthorizationManager authorizationManager, IdentityUserManager userManager, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _applicationManager = applicationManager;
            _tokenManager = tokenManager;
            _currentTenant = currentTenant;
            _authorizationManager = authorizationManager;
            _userManager = userManager;
            _configuration = configuration;
                        _googleClientId = configuration["Authentication:Google:ClientId"] ?? string.Empty;

        }
        public async Task<object> GoogleLoginAsync(GoogleLoginDto input)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { _googleClientId } };

            var payload = await GoogleJsonWebSignature.ValidateAsync(input.IdToken, settings);

            if (payload == null || string.IsNullOrEmpty(payload.Email))
                throw new UnauthorizedAccessException("Invalid Google Token");


            //var payload = await GoogleJsonWebSignature.ValidateAsync(input.IdToken);

            // You can check payload.EmailVerified, payload.Audience, etc.
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), payload.Email, payload.Email, _currentTenant.Id);
                await _userManager.CreateAsync(user);
            }

            var token = await GenerateAccessTokenAsync(user.Id);
            return (new { access_token = token });
        }
        private async Task<string> GenerateAccessTokenAsync(Guid userId, string clientId = "MindflowAI_App")
        {
            using (_currentTenant.Change(null)) // optional: adjust for multi-tenant
            {
                var application = await _applicationManager.FindByClientIdAsync(clientId);
                if (application == null)
                {
                    throw new Exception("Invalid client_id.");
                }

                var identity = new ClaimsIdentity(OpenIddictConstants.Schemes.Bearer);
                identity.AddClaim(OpenIddictConstants.Claims.Subject, userId.ToString());
                identity.SetDestinations(_ => new[] { Destinations.AccessToken });

                var principal = new ClaimsPrincipal(identity);
                principal.SetScopes("MindflowAI"); // or whatever scope you configured

                var tokenDescriptor = new OpenIddictTokenDescriptor
                {
                    Principal = principal,
                    Type = OpenIddictConstants.TokenTypes.Bearer,
                    // Optional:
                    // ApplicationId = your client_id,
                    // ExpirationDate = DateTimeOffset.UtcNow.AddHours(1),
                };

                var token = await _tokenManager.CreateAsync(tokenDescriptor);
                return await _tokenManager.GetIdAsync(token);
            }
        }

        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            yield return Destinations.AccessToken;

            if (claim.Type == Claims.Name || claim.Type == Claims.Email)
                yield return Destinations.IdentityToken;
        }
    }

}
