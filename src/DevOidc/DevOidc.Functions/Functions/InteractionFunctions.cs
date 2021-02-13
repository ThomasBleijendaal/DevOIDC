using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Extensions;
using DevOidc.Core.Models;
using DevOidc.Functions.Models.Request;
using DevOidc.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    public class InteractionFunctions
    {
        private readonly ITenantService _tenantService;
        private readonly ISessionService _sessionService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IJwtProvider _jwtProvider;

        public InteractionFunctions(
            ITenantService tenantService,
            ISessionService sessionService,
            IScopeProvider scopeProvider,
            IClaimsProvider claimsProvider,
            IJwtProvider jwtProvider)
        {
            _tenantService = tenantService;
            _sessionService = sessionService;
            _scopeProvider = scopeProvider;
            _claimsProvider = claimsProvider;
            _jwtProvider = jwtProvider;
        }

        [FunctionName(nameof(AuthorizeAsync))]
        public async Task<HttpResponseMessage> AuthorizeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/authorize")] HttpRequest req,
            string tenantId)
        {
            var requestModel = req.BindModelToQuery<OidcAuthorizeRequestModel>();
            if (requestModel.Prompt == "none")
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var message = requestModel.Error switch
            {
                "invalid_request" => FormView.Error("The configuration send in POST is incorrect."),
                "invalid_login" => FormView.Error("The username or password is incorrect, or the user does not have access to the client."),
                _ => default
            };

            string body;
            if (string.IsNullOrWhiteSpace(requestModel.ClientId))
            {
                message = FormView.Error($"Missing <code>client_id</code>.");
            }
            else if (await _tenantService.GetClientAsync(tenantId, requestModel.ClientId) is not ClientDto tenantClient)
            {
                message = FormView.Error($"Client <code>{requestModel.ClientId}</code> not found.");
            }
            else if (_scopeProvider.GetCustomScopes(requestModel.Scopes).Except(tenantClient.Scopes.Select(x => x.ScopeId)) is IEnumerable<string> unsupportedScopes && unsupportedScopes.Any())
            {
                message = FormView.Error($"Client <code>{requestModel.ClientId}</code> does not support scope <code>{string.Join(" ", unsupportedScopes)}</code>.");
            }
            else if (string.IsNullOrWhiteSpace(requestModel.RedirectUri) || !tenantClient.RedirectUris.Contains(requestModel.RedirectUri))
            {
                message = FormView.Error($"Client <code>{requestModel.ClientId}</code> does have a redirect uri <code>{requestModel.RedirectUri}</code>.");
            }
            else if (requestModel.ResponseType != "code" && requestModel.ResponseType != "id_token")
            {
                message = FormView.Error($"Client <code>{requestModel.ClientId}</code> only supports <code>response_type=code</code> or <code>response_type=id_token</code>.");
            }
            else if (requestModel.ResponseMode != "fragment" && requestModel.ResponseMode != "form_post" && requestModel.ResponseMode != "query")
            {
                message = FormView.Error($"Client <code>{requestModel.ClientId}</code> only supports <code>response_type=form_post</code> or <code>response_type=fragment</code> or <code>response_type=query</code>.");
            }

            body = FormView.RenderForm(
                new Dictionary<string, string?>
                {
                    { "client_id", requestModel.ClientId },
                    { "redirect_uri", requestModel.RedirectUri },
                    { "scope", requestModel.Scope },
                    { "audience", requestModel.Audience },
                    { "response_mode", requestModel.ResponseMode },
                    { "response_type", requestModel.ResponseType },
                    { "state", requestModel.State },
                    { "nonce", requestModel.Nonce },
                },
                new Dictionary<string, string>
                {
                    { "username", "text" },
                    { "password", "password" }
                },
                "Sign in",
                message);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(FormView.RenderHtml(body), Encoding.UTF8, "text/html")
            };
        }

        [FunctionName(nameof(AuthorizePostbackAsync))]
        public async Task<HttpResponseMessage> AuthorizePostbackAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/authorize")] HttpRequest req,
            string tenantId)
        {
            var requestModel = req.BindModelToForm<OidcAuthorizeRequestModel>();

            if (string.IsNullOrWhiteSpace(requestModel.ClientId) ||
                string.IsNullOrWhiteSpace(requestModel.RedirectUri) ||
                string.IsNullOrWhiteSpace(requestModel.UserName) ||
                string.IsNullOrWhiteSpace(requestModel.Password) ||
                string.IsNullOrWhiteSpace(requestModel.ResponseType) ||
                await _tenantService.GetTenantAsync(tenantId) is not TenantDto tenant ||
                await _tenantService.GetClientAsync(tenantId, requestModel.ClientId) is not ClientDto client ||
                !client.RedirectUris.Contains(requestModel.RedirectUri))
            {
                return RedirectToLogin("invalid_request", "Incorrect configuration was posted to callback.", req, requestModel);
            }

            var scope = client.Scopes.FirstOrDefault(x => requestModel.Scopes.Contains(x.ScopeId))
                ?? new ScopeDto { ScopeId = requestModel.ClientId };

            var audience = !string.IsNullOrWhiteSpace(requestModel.Audience) ? requestModel.Audience : scope.ScopeId;

            var user = await _tenantService.AuthenticateUserAsync(tenantId, requestModel.ClientId, requestModel.UserName, requestModel.Password);
            if (user == null)
            {
                return RedirectToLogin("invalid_login", "Username or password is incorrect, or user does not have access to this client.", req, requestModel);
            }

            if (requestModel.ResponseType == "id_token")
            {
                return await RedirectIdTokenToClientAppAsync(req, requestModel, tenant, user, client, scope);
            }
            else if (requestModel.ResponseType == "code")
            {
                return await RedirectCodeToClientAppAsync(requestModel, tenant, user, client, scope, audience);
            }
            else
            {
                return RedirectToLogin("invalid_request", "Response type not supported", req, requestModel);
            }
        }

        [FunctionName(nameof(SignOut))]
        public HttpResponseMessage SignOut(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/logout")] HttpRequest req,
            string tenantId)
        {
            var model = req.BindModelToQuery<OidcLogoutRequestModel>();

            return RedirectToClientApp(model);
        }

        private static HttpResponseMessage RedirectToLogin(string error, string errorDescription, HttpRequest req, OidcAuthorizeRequestModel requestModel)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Found);
            response.Headers.Location = new Uri(new Uri($"{req.Scheme}://{req.Host}{req.Path}"), $"?client_id={requestModel.ClientId}&redirect_uri={requestModel.RedirectUri}&scope={requestModel.Scope}&response_type={requestModel.ResponseType}&state={requestModel.State}&nonce={requestModel.Nonce}&error={error}&error_description={HttpUtility.UrlEncode(errorDescription)}");
            return response;
        }

        private async Task<HttpResponseMessage> RedirectIdTokenToClientAppAsync(HttpRequest req, OidcAuthorizeRequestModel requestModel, TenantDto tenant, UserDto user, ClientDto client, ScopeDto scope)
        {
            var encryptionProvider = await _tenantService.GetEncryptionProviderAsync(tenant.TenantId);
            if (encryptionProvider == null)
            {
                return RedirectToLogin("invalid_request", "Incorrect encryption provider for tentant", req, requestModel);
            }

            var idTokenClaims = _claimsProvider.CreateIdTokenClaims(user, client, scope.ScopeId, requestModel.Nonce);
            var idToken = _jwtProvider.CreateJwt(idTokenClaims, tenant.TokenLifetime, encryptionProvider);

            if (requestModel.ResponseMode == "form_post")
            {
                var body = FormView.RenderForm(
                    new Dictionary<string, string?>
                    {
                        { "state", requestModel.State },
                        { "id_token", idToken },
                    },
                    new Dictionary<string, string>(),
                    "Send code to application to resume flow",
                    default,
                    requestModel.RedirectUri);

                var repsonse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(FormView.RenderHtml(body), Encoding.UTF8, "text/html")
                };

                return repsonse;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.Found);
                response.Headers.Location = new Uri(new Uri(requestModel.RedirectUri!), $"{(requestModel.ResponseMode == "fragment" ? "#" : "?")}id_token={idToken}&state={requestModel.State}");
                return response;
            }
        }

        private async Task<HttpResponseMessage> RedirectCodeToClientAppAsync(OidcAuthorizeRequestModel requestModel, TenantDto tenant, UserDto user, ClientDto client, ScopeDto scope, string audience)
        {
            var code = await _sessionService.CreateSessionAsync(tenant.TenantId, user, client, scope.ScopeId, requestModel.Scopes, audience, requestModel.Nonce);

            if (requestModel.ResponseMode == "form_post")
            {
                var body = FormView.RenderForm(
                    new Dictionary<string, string?>
                    {
                        { "state", requestModel.State },
                        { "code", code },
                    },
                    new Dictionary<string, string>(),
                    "Send code to application to resume flow",
                    default,
                    requestModel.RedirectUri);

                var repsonse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(FormView.RenderHtml(body), Encoding.UTF8, "text/html")
                };

                return repsonse;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.Found);
                response.Headers.Location = new Uri(new Uri(requestModel.RedirectUri!), $"{(requestModel.ResponseMode == "fragment" ? "#" : "?")}code={code}&state={requestModel.State}");
                return response;
            }
        }

        private HttpResponseMessage RedirectToClientApp(OidcLogoutRequestModel logoutModel)
        {
            var body = FormView.RenderForm(
                new Dictionary<string, string?>
                {
                    { "state", logoutModel.State }
                },
                new Dictionary<string, string>(),
                "Click to sign out",
                string.IsNullOrWhiteSpace(logoutModel.LogoutRedirectUri) ? "Redirect Url is empty!" : default,
                logoutModel.LogoutRedirectUri,
                method: "get");

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(FormView.RenderHtml(body), Encoding.UTF8, "text/html")
            };

            return response;
        }
    }
}
