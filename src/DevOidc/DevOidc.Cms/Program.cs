using System;
using System.Threading.Tasks;
using DevOidc.Cms.Components.Editors;
using DevOidc.Cms.Components.Login;
using DevOidc.Cms.Components.Panes;
using DevOidc.Cms.Components.Sections;
using DevOidc.Cms.Core.Models;
using DevOidc.Cms.Handlers;
using DevOidc.Cms.Models;
using DevOidc.Core.Models.Dtos;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Repositories.ApiBridge;
using RapidCMS.UI.Components.Buttons;

namespace DevOidc.Cms
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var apiUri = new Uri(builder.Configuration["Uris:Api"]);

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            builder.Services.AddAuthorizationCore();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

                options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration["AzureAD:Scope"]);
                options.ProviderOptions.LoginMode = "redirect";
            });

            builder.Services.AddRapidCMSApiTokenAuthorization(sp =>
            {
                var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
                handler.ConfigureHandler(new[] { apiUri.ToString() });
                return handler;
            });

            builder.Services.AddRapidCMSAuthenticatedApiRepository<ApiMappedRepository<TenantCmsModel, TenantDto>, AuthorizationMessageHandler>(apiUri);
            builder.Services.AddRapidCMSAuthenticatedApiRepository<ApiMappedRepository<UserCmsModel, UserDto>, AuthorizationMessageHandler>(apiUri);
            builder.Services.AddRapidCMSAuthenticatedApiRepository<ApiMappedRepository<ClientCmsModel, ClientDto>, AuthorizationMessageHandler>(apiUri);

            builder.Services.AddScoped<ClaimTenantButtonHandler>();
            builder.Services.AddScoped<ResetPasswordButtonHandler>();

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.Advanced.SemaphoreCount = 5;
                config.SetSiteName("DevOIDC CMS");

                config.SetCustomLoginStatus(typeof(LoginStatus));
                config.SetCustomLoginScreen(typeof(LoginScreen));

                config.Dashboard.AddSection("tenant");
                config.Dashboard.AddSection(typeof(OidcHelp));

                config.AddCollection<TenantCmsModel, ApiMappedRepository<TenantCmsModel, TenantDto>>("tenant", "Tenants", config =>
                {
                    config.SetTreeView(EntityVisibilty.Visible, CollectionRootVisibility.Hidden, x => x.Name, showEntitiesOnStartup: true);

                    config.AddDataView("My", x => true);
                    config.AddDataView("Others", x => true);

                    config.SetListView(x =>
                    {
                        x.SetSearchBarVisibility(false);

                        x.AddDefaultButton(DefaultButtonType.New);

                        x.AddRow(row =>
                        {
                            row.AddField(x => x.Id).SetName("Tenant Id").SetType(DisplayType.Pre);
                            row.AddField(x => x.OwnerName).SetName("Owner");
                            row.AddField(x => x.Name);
                            row.AddField(x => x.Description);

                            row.AddDefaultButton(DefaultButtonType.Edit);
                        });
                    });

                    config.SetNodeEditor(x =>
                    {
                        x.AddDefaultButton(DefaultButtonType.Return);
                        x.AddDefaultButton(DefaultButtonType.Up);
                        x.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                        x.AddCustomButton<ClaimTenantButtonHandler>(typeof(DefaultButton), "Take ownership of tenant", "Crown");
                        x.AddDefaultButton(DefaultButtonType.Delete);

                        x.AddSection(section =>
                        {
                            section.AddField(x => x.Id).SetName("Tenant Id").SetType(DisplayType.Pre);
                            section.AddField(x => x.OwnerName).SetName("Owner").SetType(DisplayType.Pre);
                            
                            section.AddField(x => $"https://devoidc.azurewebsites.net/{x.Id}/.well-known/openid-configuration").SetType(DisplayType.Pre).SetName("Metadata endpoint");
                            section.AddField(x => $"https://devoidc.azurewebsites.net/{x.Id}").SetType(DisplayType.Pre).SetName("Authority");
                            section.AddField(x => $"https://devoidc.azurewebsites.net/{x.Id}").SetType(DisplayType.Pre).SetName("Issuer / token source");

                            section.AddField(x => x.Name).DisableWhen((m, s) => s == EntityState.IsExisting);
                            section.AddField(x => x.Description).DisableWhen((m, s) => s == EntityState.IsExisting);
                            section.AddField(x => x.TokenLifetimeSeconds).DisableWhen((m, s) => s == EntityState.IsExisting)
                                .SetName("Token lifetime").SetDescription("In seconds").SetType(EditorType.Numeric);
                        });

                        x.AddSection(section =>
                        {
                            section.VisibleWhen((m, s) => s == EntityState.IsExisting);

                            section.AddSubCollectionList("user");
                            section.AddSubCollectionList("client");
                        });
                    });

                    config.AddSubCollection<UserCmsModel, ApiMappedRepository<UserCmsModel, UserDto>>("user", "Contact", "Green10", "Users", config =>
                    {
                        config.SetTreeView(x => x.FullName);

                        config.SetListEditor(x =>
                        {
                            x.SetSearchBarVisibility(false);

                            x.AddDefaultButton(DefaultButtonType.New);

                            x.AddSection(section =>
                            {
                                section.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                section.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                section.AddDefaultButton(DefaultButtonType.Edit, label: "Extra claims and clients");
                                section.AddCustomButton<ResetPasswordButtonHandler>(typeof(DefaultButton), label: "Reset password", icon: "key");

                                section.AddField(x => x.Id).SetName("User Id").SetType(DisplayType.Pre);
                                section.AddField(x => x.FullName).SetName("Full name");
                                section.AddField(x => x.UserName).SetName("User name");
                                section.AddField(x => x.Password).DisableWhen((m, e) => true).VisibleWhen((m, e) => e == EntityState.IsExisting);
                            });
                        });

                        config.SetNodeEditor(x =>
                        {
                            x.AddDefaultButton(DefaultButtonType.Up);
                            x.AddDefaultButton(DefaultButtonType.Return);
                            x.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                            x.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                            x.AddDefaultButton(DefaultButtonType.Delete);
                            x.AddCustomButton<ResetPasswordButtonHandler>(typeof(DefaultButton), label: "Reset password", icon: "key");

                            x.AddSection(section =>
                            {
                                section.AddField(x => x.Id).SetName("User Id").SetType(DisplayType.Pre);
                                section.AddField(x => x.FullName).SetName("Full name");
                                section.AddField(x => x.UserName).SetName("User name");
                                section.AddField(x => x.Password).DisableWhen((m, e) => true);
                                section.AddField(x => x.AccessTokenExtraClaims).SetType(typeof(ClaimEditor)).SetName("Extra claims in Access Token");
                                section.AddField(x => x.IdTokenExtraClaims).SetType(typeof(ClaimEditor)).SetName("Extra claims in ID Token");
                                section.AddField(x => x.UserInfoExtraClaims).SetType(typeof(ClaimEditor)).SetName("Extra claims in OIDC UserInfo");
                            });

                            x.AddSection(section =>
                            {
                                section.AddField(x => x.Clients)
                                    .SetType(EditorType.MultiSelect)
                                    .SetDescription("User has permission to request tokens using these clients")
                                    .SetCollectionRelation<ClientCmsModel>("client", relation =>
                                    {
                                        relation.SetElementIdProperty(x => x.Id);
                                        relation.SetElementDisplayProperties(x => x.Id, x => x.Name);
                                        relation.SetRepositoryParent(parent => parent);
                                    });
                            });
                        });
                    });

                    config.AddSubCollection<ClientCmsModel, ApiMappedRepository<ClientCmsModel, ClientDto>>("client", "Devices2", "Red10", "Clients", config =>
                    {
                        config.SetTreeView(x => x.Name);

                        config.SetListEditor(x =>
                        {
                            x.SetSearchBarVisibility(false);

                            x.AddDefaultButton(DefaultButtonType.New);

                            x.AddSection(section =>
                            {
                                section.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                                section.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                                section.AddDefaultButton(DefaultButtonType.Edit, label: "Edit all properties");

                                section.AddField(x => x.Id).SetName("Client Id").SetType(DisplayType.Pre).SetOrderByExpression(x => x.Id, OrderByType.None);
                                section.AddField(x => x.Name).SetOrderByExpression(x => x.Name, OrderByType.Ascending);
                                section.AddField(x => x.RedirectUris)
                                    .SetType(typeof(ListEditor))
                                    .DisableWhen((m, s) => true)
                                    .SetName("Redirect URIs");
                            });
                        });

                        config.SetNodeEditor(x =>
                        {
                            x.AddDefaultButton(DefaultButtonType.Up);
                            x.AddDefaultButton(DefaultButtonType.Return);
                            x.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                            x.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);
                            x.AddDefaultButton(DefaultButtonType.Delete);

                            x.AddSection(section =>
                            {
                                section.AddField(x => x.Id).SetName("Client Id").SetType(DisplayType.Pre);
                                section.AddField(x => x.Name);
                                section.AddField(x => x.AccessTokenExtraClaims).SetType(typeof(ClaimEditor)).SetName("Extra claims in Access Token");
                                section.AddField(x => x.IdTokenExtraClaims).SetType(typeof(ClaimEditor)).SetName("Extra claims in ID Token");
                                section.AddField(x => x.Scopes).SetType(typeof(ScopeEditor)).SetName("Allowed scopes");
                            });

                            x.AddSection(section =>
                            {
                                section.AddField(x => x.RedirectUris).SetType(typeof(ListEditor)).SetName("Redirect URIs");

                                section.AddPaneButton(typeof(RedirectUrlPane), "Add Redirect URI using wizard", "add");
                            });
                        });
                    });
                });
            });

            var host = builder.Build();

            host.Services.GetRequiredService<ICms>().IsDevelopment = true;

            await host.RunAsync();
        }
    }
}
