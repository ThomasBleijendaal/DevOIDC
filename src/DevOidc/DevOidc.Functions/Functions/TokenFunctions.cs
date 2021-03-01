using System;
using System.Threading.Tasks;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Exceptions;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Extensions;
using DevOidc.Functions.Models.Request;
using DevOidc.Functions.Models.Response;
using DevOidc.Functions.Responses;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    public class TokenFunctions
    {
        private readonly IOidcHandler<IOidcTokenRequest, IOidcToken> _oidcHandler;

        public TokenFunctions(IOidcHandler<IOidcTokenRequest, IOidcToken> oidcHandler)
        {
            _oidcHandler = oidcHandler;
        }

        [FunctionName(nameof(GetTokenAsync))]
        public async Task<HttpResponseData> GetTokenAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/token")] HttpRequestData req)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            var requestModel = req.BindModelToForm<OidcTokenRequestModel>();

            try
            {
                var response = await _oidcHandler.HandleAsync(requestModel.GetRequest(tenantId));

                return Response.Json(new TokenResponseModel
                {
                    TokenType = response.TokenType,
                    ExpiresIn = response.ExpiresIn,
                    ExtExpiresIn = response.ExpiresIn,
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
                    IdToken = response.IdToken,
                    Scope = response.Scope
                });
            }
            catch (InvalidRequestException ex)
            {
                return Response.Json(new ErrorResonseModel
                {
                    Error = "invalid_request",
                    ErrorDescription = ex.Message
                });
            }
            catch (InvalidGrantException ex)
            {
                return Response.Json(new ErrorResonseModel
                {
                    Error = "invalid_grant",
                    ErrorDescription = ex.Message
                });
            }
            catch (Exception)
            {
                return Response.BadRequest();
            }
        }
    }
}
