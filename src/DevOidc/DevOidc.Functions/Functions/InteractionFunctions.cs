﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using DevOidc.Business.Abstractions;
using DevOidc.Business.Abstractions.Request;
using DevOidc.Core.Exceptions;
using DevOidc.Core.Models.Dtos;
using DevOidc.Functions.Authentication;
using DevOidc.Functions.Extensions;
using DevOidc.Functions.Models.Request;
using DevOidc.Functions.Responses;
using DevOidc.Functions.Views;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Pipeline;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    public class InteractionFunctions
    {
        private readonly IOidcHandler<IOidcTokenRequest, IOidcAuthorization> _authorizationHandler;
        private readonly ITenantService _tenantService;
        private readonly IScopeProvider _scopeProvider;

        public InteractionFunctions(
            IOidcHandler<IOidcTokenRequest, IOidcAuthorization> authorizationHandler,
            ITenantService tenantService,
            IScopeProvider scopeProvider)
        {
            _authorizationHandler = authorizationHandler;
            _tenantService = tenantService;
            _scopeProvider = scopeProvider;
        }

        [FunctionName(nameof(AuthorizeAsync))]
        public async Task<HttpResponseData> AuthorizeAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/authorize")] HttpRequestData req)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            var userName = default(string);
            if (req.Headers.TryGetValue("cookie", out var cookies))
            {
                var userNameCookie = cookies.Split(";", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.StartsWith("username"));
                userName = userNameCookie?.Replace("username=", "").Trim();
            }

            var requestModel = req.BindModelToQuery<OidcAuthorizeRequestModel>();
            if (requestModel.Prompt == "none")
            {
                return Response.NoContent();
            }

            var message = requestModel.Error switch
            {
                "invalid_request" => FormView.Error("The configuration send in POST is incorrect."),
                "invalid_login" => FormView.Error("The username or password is incorrect, or the user does not have access to the client."),
                _ => default
            };

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

            return Response.Html(FormView.RenderHtml(LogInForm(requestModel, message, userName)));
        }

        [FunctionName(nameof(AuthorizePostbackAsync))]
        public async Task<HttpResponseData> AuthorizePostbackAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/authorize")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            var requestModel = req.BindModelToForm<OidcAuthorizeRequestModel>();

            try
            {
                var request = requestModel.GetRequest(tenantId);
                var authorizationResponse = await _authorizationHandler.HandleAsync(request);

                var response = requestModel.ResponseMode == "form_post" ? PostToReplyUrlForm(requestModel, authorizationResponse.Type, authorizationResponse.Value)

                    // web sites etc
                    : (requestModel.RedirectUri?.StartsWith("http") ?? false) ? RedirectToReplyUrl(requestModel, authorizationResponse.Type, authorizationResponse.Value)

                    // native clients 
                    : PostToCallbackUrlForm(requestModel, context, authorizationResponse.Type, authorizationResponse.Value);

                response.Headers.Add("Set-Cookie", $"username={requestModel.UserName}");

                return response;
            }
            catch (InvalidRequestException ex)
            {
                return RedirectToLogin(context, tenantId, "invalid_request", ex.Message, requestModel);
            }
            catch (Exception ex)
            {
                return RedirectToLogin(context, tenantId, ex.Message, ex.Message, requestModel);
            }
        }

        [FunctionName(nameof(AuthorizePostbackCallbackAsync))]
        public async Task<HttpResponseData> AuthorizePostbackCallbackAsync(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/authorize/callback")] HttpRequestData req, FunctionExecutionContext context)
        {
            if (!req.Params.TryGetValue("tenantId", out var tenantId))
            {
                return Response.BadRequest();
            }

            var requestModel = req.BindModelToQuery<OidcAuthorizeCallbackRequestModel>();

            if (string.IsNullOrWhiteSpace(requestModel.ClientId) ||
                string.IsNullOrWhiteSpace(requestModel.RedirectUri) ||
                string.IsNullOrWhiteSpace(requestModel.Scope) ||
                await _tenantService.GetTenantAsync(tenantId) is not TenantDto tenant ||
                await _tenantService.GetClientAsync(tenantId, requestModel.ClientId) is not ClientDto client ||
                !client.RedirectUris.Contains(requestModel.RedirectUri))
            {
                return RedirectToLogin(context, tenantId, "invalid_request", "Incorrect configuration was posted to callback.", requestModel);
            }

            var type = string.IsNullOrWhiteSpace(requestModel.Code) ? "id_token" : "code";
            var value = type == "id_token" ? requestModel.IdToken : requestModel.Code;

            return RedirectToReplyUrl(requestModel, type, value);
        }

        [FunctionName(nameof(SignOut))]
        public HttpResponseData SignOut(
            [AllowAnonymous][HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/logout")] HttpRequestData req)
            => RedirectToClientApp(req.BindModelToQuery<OidcLogoutRequestModel>());

        private static HttpResponseData RedirectToLogin(FunctionExecutionContext context, string tenantId, string error, string errorDescription, OidcRequestModel request)
            => Response.Found($"{context.GetBaseUri(tenantId)}{tenantId}/authorize?error={error}&error_description={HttpUtility.UrlEncode(errorDescription)}&{GenerateOidcData(request)}");

        private static string GenerateOidcData(OidcRequestModel m)
            => $"client_id={m.ClientId}&scope={m.Scope}&redirect_uri={HttpUtility.UrlEncode(m.RedirectUri)}&response_type={m.ResponseType}&response_mode={m.ResponseMode}";

        private static string LogInForm(OidcAuthorizeRequestModel requestModel, string? message, string? userName)
            => FormView.RenderForm(
                requestModel.LogInFormData(),
                new Dictionary<string, (string, string?)>
                {
                    { "username", ("text", userName) },
                    { "password", ("password", null) }
                },
                "Sign in",
                message);

        private static HttpResponseData RedirectToClientApp(OidcLogoutRequestModel logoutModel)
            => Response.Html(FormView.RenderHtml(FormView.RenderForm(
                new Dictionary<string, string?>
                {
                    { "state", logoutModel.State }
                },
                new Dictionary<string, (string, string?)>(),
                "Click to sign out",
                string.IsNullOrWhiteSpace(logoutModel.LogoutRedirectUri) ? "Redirect Url is empty!" : default,
                logoutModel.LogoutRedirectUri,
                method: "get")));

        private static HttpResponseData PostToCallbackUrlForm(OidcAuthorizeRequestModel requestModel, FunctionExecutionContext context, string type, string? value)
            => Response.Html(FormView.RenderHtml(FormView.RenderForm(
                new Dictionary<string, string?>
                {
                    { "client_id", requestModel.ClientId },
                    { "redirect_uri", requestModel.RedirectUri },
                    { type, value },
                    { "scope", requestModel.Scope },
                    { "state", requestModel.State },
                    { "session_state", Guid.NewGuid().ToString() },
                    { "response_mode", requestModel.ResponseMode }
                },
                new Dictionary<string, (string, string?)>(),
                $"Send {type} to application to resume flow",
                default,
                url: $"{context.GetBaseUri()}/callback",
                method: "get")));

        private static HttpResponseData PostToReplyUrlForm(OidcAuthorizeRequestModel requestModel, string type, string? value)
            => Response.Html(FormView.RenderHtml(FormView.RenderForm(
                new Dictionary<string, string?>
                {
                    { "state", requestModel.State },
                    { type, value },
                },
                new Dictionary<string, (string, string?)>(),
                $"Send {type} to application to resume flow",
                default,
                requestModel.RedirectUri)));

        private static HttpResponseData RedirectToReplyUrl(OidcRequestModel requestModel, string type, string? value)
            => Response.Found($"{requestModel.RedirectUri}{(requestModel.ResponseMode == "fragment" ? "#" : "?")}{GenerateQueryData(requestModel, type, value)}");

        private static string GenerateQueryData(OidcRequestModel requestModel, string type, string? value)
        {
            var data = $"{type}={value}";

            if (!string.IsNullOrWhiteSpace(requestModel.State))
            {
                data += $"&state={requestModel.State}";
            }
            if (!string.IsNullOrWhiteSpace(requestModel.Scope))
            {
                data += $"&scope={HttpUtility.UrlEncode(requestModel.Scope)}";
            }

            data += $"&session_state={requestModel.SessionState}";

            return data;
        }
    }
}
