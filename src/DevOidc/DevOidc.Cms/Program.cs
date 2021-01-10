using System;
using System.Threading.Tasks;
using DevOidc.Cms.ButtonHandlers;
using DevOidc.Cms.Components.Editors;
using DevOidc.Cms.Components.Panes;
using DevOidc.Cms.Models;
using DevOidc.Cms.Repositories;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Enums;
using RapidCMS.UI.Components.Buttons;

namespace DevOidc.Cms
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAuthorizationCore();
            builder.Services.AddHttpClient("cms", (httpClient) =>
            {
                httpClient.BaseAddress = new Uri("http://localhost:7071/cms/");
            });

            builder.Services.AddTransient<ClientRepository>();
            builder.Services.AddTransient<TenantRepository>();
            builder.Services.AddTransient<UserRepository>();

            builder.Services.AddTransient<ResetPasswordButtonHandler>();

            builder.Services.AddRapidCMSWebAssembly(config =>
            {
                config.SetSiteName("DevOIDC");

                config.AllowAnonymousUser();

                config.Dashboard.AddSection("tenant");

                config.AddCollection<TenantCmsModel, TenantRepository>("tenant", "Tenants", config =>
                {
                    config.SetTreeView(EntityVisibilty.Visible, CollectionRootVisibility.Hidden, x => x.Name, showEntitiesOnStartup: true);

                    config.SetListView(x =>
                    {
                        x.SetSearchBarVisibility(false);

                        x.AddDefaultButton(DefaultButtonType.New);

                        x.AddRow(row =>
                        {
                            row.AddField(x => x.Id).SetType(DisplayType.Pre);
                            row.AddField(x => x.OwnerName);
                            row.AddField(x => x.Name);
                            row.AddField(x => x.Description);

                            row.AddDefaultButton(DefaultButtonType.Edit);
                        });
                    });

                    config.SetNodeEditor(x =>
                    {
                        x.AddDefaultButton(DefaultButtonType.Up);
                        x.AddDefaultButton(DefaultButtonType.SaveNew);
                        x.AddDefaultButton(DefaultButtonType.Delete);

                        x.AddSection(section =>
                        {
                            section.AddField(x => x.Id).SetType(DisplayType.Pre);
                            section.AddField(x => x.Name).DisableWhen((m, s) => s == EntityState.IsExisting);
                            section.AddField(x => x.Description).DisableWhen((m, s) => s == EntityState.IsExisting);
                            section.AddField(x => x.TokenLifetime.TotalSeconds).SetName("Token lifetime").SetDescription("In seconds").SetType(DisplayType.Label);
                        });

                        x.AddSection(section =>
                        {
                            section.VisibleWhen((m, s) => s == EntityState.IsExisting);

                            section.AddSubCollectionList("user");
                            section.AddSubCollectionList("client");
                        });
                    });

                    config.AddSubCollection<UserCmsModel, UserRepository>("user", "Users", config =>
                    {
                        config.SetTreeView(x => x.FullName);

                        config.SetListEditor(x =>
                        {
                            x.SetSearchBarVisibility(false);

                            x.AddDefaultButton(DefaultButtonType.New);

                            x.AddSection(section =>
                            {
                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                                section.AddDefaultButton(DefaultButtonType.Edit, label: "Extra claims and clients");
                                section.AddCustomButton<ResetPasswordButtonHandler>(typeof(DefaultButton), label: "Reset password", icon: "key");

                                section.AddField(x => x.FullName).SetName("Full name");
                                section.AddField(x => x.UserName).SetName("User name");
                                section.AddField(x => x.Password).DisableWhen((m, e) => true).VisibleWhen((m, e) => e == EntityState.IsExisting);
                            });
                        });

                        config.SetNodeEditor(x =>
                        {
                            x.AddDefaultButton(DefaultButtonType.Up);
                            x.AddDefaultButton(DefaultButtonType.SaveExisting);
                            x.AddDefaultButton(DefaultButtonType.SaveNew);
                            x.AddDefaultButton(DefaultButtonType.Delete);
                            x.AddCustomButton<ResetPasswordButtonHandler>(typeof(DefaultButton), label: "Reset password", icon: "key");

                            x.AddSection(section =>
                            {
                                section.AddField(x => x.FullName).SetName("Full name");
                                section.AddField(x => x.UserName).SetName("User name");
                                section.AddField(x => x.Password).DisableWhen((m, e) => true);
                                section.AddField(x => x.ExtraClaims).SetType(typeof(ClaimEditor)).SetName("Extra claims").SetDescription("In JWT token");
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

                    config.AddSubCollection<ClientCmsModel, ClientRepository>("client", "Clients", config =>
                    {
                        config.SetTreeView(x => x.Name);

                        config.SetListEditor(x =>
                        {
                            x.SetSearchBarVisibility(false);

                            x.AddDefaultButton(DefaultButtonType.New);

                            x.AddSection(section =>
                            {
                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                                section.AddDefaultButton(DefaultButtonType.Edit, label: "Edit all properties");

                                section.AddField(x => x.Id).SetType(DisplayType.Pre);
                                section.AddField(x => x.Name);
                                section.AddField(x => x.RedirectUris)
                                    .SetType(typeof(ListEditor))
                                    .DisableWhen((m, s) => true)
                                    .SetName("Redirect URIs");
                            });
                        });

                        config.SetNodeEditor(x =>
                        {
                            x.AddDefaultButton(DefaultButtonType.Up);
                            x.AddDefaultButton(DefaultButtonType.SaveExisting);
                            x.AddDefaultButton(DefaultButtonType.SaveNew);
                            x.AddDefaultButton(DefaultButtonType.Delete);

                            x.AddSection(section =>
                            {
                                section.AddField(x => x.Id).SetType(DisplayType.Pre);
                                section.AddField(x => x.Name);
                                section.AddField(x => x.ExtraClaims).SetType(typeof(ClaimEditor)).SetName("Extra claims").SetDescription("In JWT token");
                                section.AddField(x => x.Scopes).SetType(typeof(ScopeEditor)).SetName("Allowed scopes");
                            });

                            x.AddSection(section =>
                            {
                                section.AddField(x => x.RedirectUris).SetType(typeof(ListEditor)).SetName("Redirect URIs");

                                section.AddPaneButton(typeof(RedirectUrlPane), "Add Redirect URI using wizard", "add", CrudType.None);
                            });
                        });
                    });
                });
            });

            await builder.Build().RunAsync();
        }
    }
}
