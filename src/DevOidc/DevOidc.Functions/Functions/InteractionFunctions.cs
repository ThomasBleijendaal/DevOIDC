using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevOidc.Core.Extensions;
using DevOidc.Core.Models;
using DevOidc.Functions.Models.Request;
using DevOidc.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DevOidc.Functions.Functions
{
    public class InteractionFunctions
    {
        private readonly ITenantService _tenantService;
        private readonly ISessionService _sessionService;

        public InteractionFunctions(
            ITenantService tenantService,
            ISessionService sessionService)
        {
            _tenantService = tenantService;
            _sessionService = sessionService;
        }

        [FunctionName(nameof(Authorize))]
        public async Task<HttpResponseMessage> Authorize(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/authorize")] HttpRequest req,
            string tenantId)
        {
            var requestModel = req.BindModelToQuery<OidcAuthorizeRequestModel>();

            var message = requestModel.Error switch
            {
                "invalid_request" => "<p>The configuration send in POST is incorrect.</p>",
                "invalid_login" => "<p>The username or password is incorrect.</p>",
                _ => default
            };

            string body;
            if (string.IsNullOrWhiteSpace(requestModel.ClientId))
            {
                body = "<p>Missing <code>client_id</code>.</p>";
            }
            else if (await _tenantService.GetClientAsync(tenantId, requestModel.ClientId) is not ClientDto tenantClient)
            {
                body = $"<p>Client <code>{requestModel.ClientId}</code> not found.</p>";
            }
            else if (requestModel.CustomScopes?.Except(tenantClient.Scopes.Select(x => x.ScopeId)) is IEnumerable<string> unsupportedScopes && unsupportedScopes.Any())
            {
                body = $"<p>Client <code>{requestModel.ClientId}</code> does not support scope <code>{string.Join(" ", unsupportedScopes)}</code>.</p>";
            }
            else if (string.IsNullOrWhiteSpace(requestModel.RedirectUri) || !tenantClient.RedirectUris.Contains(requestModel.RedirectUri))
            {
                body = $"<p>Client <code>{requestModel.ClientId}</code> does have a redirect uri <code>{requestModel.RedirectUri}</code>.</p>";
            }
            else if (requestModel.ResponseType != "code")
            {
                body = $"<p>Client <code>{requestModel.ClientId}</code> only supports <code>response_type=code</code>.</p>";
            }
            else
            {
                body = $@"<form method=""post"">
<fieldset>
<legend>Settings</legend>
<label>client_id:</label>
<input readonly name=""client_id"" value=""{requestModel.ClientId}"" />
<br />
<label>redirect_uri:</label>
<input readonly name=""redirect_uri"" value=""{requestModel.RedirectUri}"" />
</br />
<label>scope:</label>
<input readonly name=""scope"" value=""{requestModel.Scope}"" />
<br />
<label>response_mode:</label>
<input readonly name=""response_mode"" value=""{requestModel.ResponseMode}"" />
</fieldset>

<fieldset>
<legend>Log in</legend>
<label>Username:</label>
<input name=""username"" />
<br />
<label>Password:</label>
<input name=""password"" type=""password"" />
<br />
<br />
<button type=""submit"">Go!</button>
</fieldset>

{message}

</form>";
            };

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(@$"<html>
<head>
<style>
* {{font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif, ""Apple Color Emoji"", ""Segoe UI Emoji"", ""Segoe UI Symbol"";}}
body {{ background: #ddd;  }}
fieldset {{ margin: 2rem; background: #fff; border: 1px solid #ccc; }}
legend {{ background: #ccc; }}
label {{ display: block; margin: .2rem; float: left; width: 25%; }}
input {{ display: block; margin: .2rem; float: right; width: 70%; }}
button {{ margin: .2rem; }}
</style>
</head>
<body>
{body}
</body>
</html>", Encoding.UTF8, "text/html")
            };
        }

        [FunctionName(nameof(AuthorizePostback))]
        public async Task<HttpResponseMessage> AuthorizePostback(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/authorize")] HttpRequest req,
            string tenantId)
        {
            var requestModel = req.BindModelToForm<OidcAuthorizeRequestModel>();

            if (string.IsNullOrWhiteSpace(requestModel.ClientId) ||
                string.IsNullOrWhiteSpace(requestModel.RedirectUri) ||
                string.IsNullOrWhiteSpace(requestModel.UserName) ||
                string.IsNullOrWhiteSpace(requestModel.Password) ||
                await _tenantService.GetClientAsync(tenantId, requestModel.ClientId) is not ClientDto client ||
                client.Scopes.FirstOrDefault(x => requestModel.Scopes.Contains(x.ScopeId)) is not ScopeDto scope ||
                !client.RedirectUris.Contains(requestModel.RedirectUri))
            {
                return RedirectToLogin("invalid_request", "Incorrect configuration was posted to callback.");
            }

            var user = await _tenantService.AuthenticateUserAsync(tenantId, requestModel.ClientId, requestModel.UserName, requestModel.Password);
            if (user == null)
            {
                return RedirectToLogin("invalid_login", "Username or password is incorrect.");
            }

            var code = await _sessionService.CreateSessionAsync(tenantId, user, client, scope, requestModel.Scope);

            return RedirectToClientApp();

            HttpResponseMessage RedirectToClientApp()
            {
                var response = new HttpResponseMessage(HttpStatusCode.Found);
                response.Headers.Add("X-Code", code);
                response.Headers.Location = new Uri(new Uri(requestModel.RedirectUri), $"{(requestModel.ResponseMode == "fragment" ? "#" : "?")}code={code}");
                return response;
            }

            HttpResponseMessage RedirectToLogin(string error, string errorDescription)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Found);
                response.Headers.Location = new Uri(new Uri($"{req.Scheme}://{req.Host}{req.Path}"), $"?client_id={requestModel.ClientId}&redirect_uri={requestModel.RedirectUri}&scope={requestModel.Scope}&response_type={requestModel.ResponseType}&error={error}&error_description={HttpUtility.UrlEncode(errorDescription)}");
                return response;
            }
        }
    }
}
