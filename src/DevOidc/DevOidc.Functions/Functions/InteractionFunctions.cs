using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevOidc.Core.Models;
using DevOidc.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DevOidc.Functions.Functions
{
    public class InteractionFunctions
    {
        private readonly ITenantService _tenantService;
        private readonly IUserSessionService _userSessionService;

        public InteractionFunctions(
            ITenantService tenantService,
            IUserSessionService userSessionService)
        {
            _tenantService = tenantService;
            _userSessionService = userSessionService;
        }

        [FunctionName(nameof(Authorize))]
        public async Task<HttpResponseMessage> Authorize(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{tenantId}/authorize")] HttpRequest req,
            string tenantId,
            ILogger log)
        {
            var clientId = req.Query["client_id"];
            var redirectUri = req.Query["redirect_uri"];
            var scopes = req.Query["scope"].ToString();
            var responseMode = req.Query["response_mode"];
            var responseType = req.Query["response_type"];
            var error = req.Query["error"].ToString();

            var customScopes = scopes?.Split(" ").Except(new[] { "openid", "offline_access" });

            var message = error switch
            {
                "invalid_request" => "<p>The configuration send in POST is incorrect.</p>",
                "invalid_login" => "<p>The username or password is incorrect.</p>",
                _ => default
            };

            var tenantClient = await _tenantService.GetClientAsync(tenantId, clientId);

            string body;
            if (tenantClient == null)
            {
                body = $"<p>Client <code>{clientId}</code> not found.</p>";
            }
            else if (customScopes?.Except(tenantClient.Scopes.Select(x => x.ScopeId)) is IEnumerable<string> unsupportedScopes && unsupportedScopes.Any())
            {
                body = $"<p>Client <code>{clientId}</code> does not support scope <code>{string.Join(" ", unsupportedScopes)}</code>.</p>";
            }
            else if (!tenantClient.RedirectUris.Contains(redirectUri))
            {
                body = $"<p>Client <code>{clientId}</code> does have a redirect uri <code>{redirectUri}</code>.</p>";
            }
            else if (responseType != "code")
            {
                body = $"<p>Client <code>{clientId}</code> only supports <code>response_type=code</code>.</p>";
            }
            else
            {
                body = $@"<form method=""post"">
<fieldset>
<legend>Settings</legend>
<label>client_id:</label>
<input readonly name=""client_id"" value=""{clientId}"" />
<br />
<label>redirect_uri:</label>
<input readonly name=""redirect_uri"" value=""{redirectUri}"" />
</br />
<label>scope:</label>
<input readonly name=""scope"" value=""{scopes}"" />
<br />
<label>response_mode:</label>
<input readonly name=""response_mode"" value=""{responseMode}"" />
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
<body>
{body}
</body>
</html>", Encoding.UTF8, "text/html")
            };
        }

        [FunctionName(nameof(AuthorizePostback))]
        public async Task<HttpResponseMessage> AuthorizePostback(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{tenantId}/authorize")] HttpRequest req,
            string tenantId,
            ILogger log)
        {
            var form = await req.ReadFormAsync();

            var clientId = form["client_id"];
            var redirectUri = form["redirect_uri"];
            var scopes = form["scope"].ToString();
            var resonseMode = form["resonse_mode"];
            var username = form["username"];
            var password = form["password"];

            var tenantClient = await _tenantService.GetClientAsync(tenantId, clientId);
            if (tenantClient == null ||
                tenantClient.Scopes.FirstOrDefault(x => scopes.Contains(x.ScopeId)) is not ScopeDto scope || 
                !tenantClient.RedirectUris.Contains(redirectUri))
            {
                return RedirectToLogin("invalid_request", "Incorrect configuration was posted to callback.");
            }

            var user = await _tenantService.AuthenticateUserAsync(tenantId, clientId, username, password);
            if (user == null)
            {
                return RedirectToLogin("invalid_login", "Username or password is incorrect.");
            }

            var code = await _userSessionService.StoreClaimsAsync(user, tenantClient, scope);

            return RedirectToClientApp();

            HttpResponseMessage RedirectToClientApp()
            {
                var response = new HttpResponseMessage(HttpStatusCode.Found);
                response.Headers.Add("X-Code", code);
                response.Headers.Location = new Uri(new Uri(redirectUri), $"{(resonseMode == "fragment" ? "#" : "?")}code={code}");
                return response;
            }

            HttpResponseMessage RedirectToLogin(string error, string errorDescription)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Found);
                response.Headers.Location = new Uri(new Uri($"{req.Scheme}://{req.Host}{req.Path}"), $"?client_id={clientId}&redirect_uri={redirectUri}&scope={scopes}&error={error}&error_description={HttpUtility.UrlEncode(errorDescription)}");
                return response;
            }
        }
    }
}
