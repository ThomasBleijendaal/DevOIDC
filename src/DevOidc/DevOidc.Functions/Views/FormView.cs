using System.Collections.Generic;
using System.Linq;

namespace DevOidc.Functions.Views
{
    public class FormView
    {
        public static string RenderForm(
            IReadOnlyDictionary<string, string?> metadata,
            IReadOnlyDictionary<string, string> inputs,
            string buttonText,
            string? messageText,
            string? url = null,
            bool hasButton = true,
            string method = "post")
        => $@"<form method=""{method}""{(string.IsNullOrEmpty(url) ? "" : $@" action=""{url}""")}>
{(string.IsNullOrWhiteSpace(messageText) ? "" : $"<div>{messageText}</div>")}
<fieldset>
<h1>DevOIDC</h1>
{string.Join("\n", metadata.Select(kv => $@"<label>{kv.Key}</label><input readonly name=""{kv.Key}"" value=""{kv.Value}"" /><br />"))}
</fieldset>
<fieldset>
{string.Join("\n", inputs.Select(kv => $@"<label>{kv.Key}</label><input name=""{kv.Key}"" type=""{kv.Value}"" /><br />"))}
<br />
{(hasButton ? $@"<button type=""submit"">{buttonText}</button>" : "")}
</fieldset>";
    
        public static string Error(string error)
            => $"<p>{error}</p>";

        public static string RenderHtml(string body)
            => @$"<html>
<head>
<title>DevOIDC</title>
<style>
* {{font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif, ""Apple Color Emoji"", ""Segoe UI Emoji"", ""Segoe UI Symbol"";}}
body {{ background: ##b6dff0;  }}
h1 {{ color: #8a8886; }}
div {{ margin: 2rem; padding: 1rem; background: #fed9cc; box-shadow: 0 25.6px 57.6px 0 rgba(0,0,0,.22),0 4.8px 14.4px 0 rgba(0,0,0,.18); }}
fieldset {{ margin: 2rem; padding: 1rem; background: #fff; box-shadow: 0 25.6px 57.6px 0 rgba(0,0,0,.22),0 4.8px 14.4px 0 rgba(0,0,0,.18); border: 0; }}
label {{ display: block; margin: .2rem; float: left; width: 15%; }}
input {{ display: block; margin: .2rem; float: right; width: 80%; border: 1px solid #979593; border-radius: 0; }}
input[readonly] {{ background-color: #edebe9; }}
button {{ margin: 1rem .2rem 1rem; background-color: #4f6bed; border: 0; padding: .7rem; color: white; cursor: pointer; border-radius: 0 !important; }}
button:hover {{ background-color: #4661d5; }}
</style>
</head>
<body>{body}</body>
</html>";
            }
}
