﻿using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace Microsoft.eShopWeb.PublicApi.AuthEndpoints
{
    public class Authenticate : BaseAsyncEndpoint
        .WithRequest<AuthenticateRequest>
        .WithResponse<AuthenticateResponse>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenClaimsService _tokenClaimsService;

        private readonly TelemetryClient _telemetry;

        public Authenticate(SignInManager<ApplicationUser> signInManager,
            ITokenClaimsService tokenClaimsService,
            TelemetryClient telemetry)
        {
            _signInManager = signInManager;
            _tokenClaimsService = tokenClaimsService;
            _telemetry = telemetry;
        }

        [HttpPost("api/authenticate")]
        [SwaggerOperation(
            Summary = "Authenticates a user",
            Description = "Authenticates a user",
            OperationId = "auth.authenticate",
            Tags = new[] { "AuthEndpoints" })
        ]
        public override async Task<ActionResult<AuthenticateResponse>> HandleAsync(AuthenticateRequest request, CancellationToken cancellationToken)
        {
            var response = new AuthenticateResponse(request.CorrelationId());

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            //var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
            var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true);

            response.Result = result.Succeeded;
            response.IsLockedOut = result.IsLockedOut;
            response.IsNotAllowed = result.IsNotAllowed;
            response.RequiresTwoFactor = result.RequiresTwoFactor;
            response.Username = request.Username;

            if (result.Succeeded)
            {
                _telemetry.TrackEvent("authenticate user");
                response.Token = await _tokenClaimsService.GetTokenAsync(request.Username);
            }
            else
            {
                _telemetry.TrackEvent("authenticate failure");
            }

            return response;
        }
    }
}
