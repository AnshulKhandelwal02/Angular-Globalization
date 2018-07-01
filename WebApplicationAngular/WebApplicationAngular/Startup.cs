using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;
using System.Security.Claims;
using WebApplicationAngular.Extensions;

namespace WebApplicationAngular
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		private string ClientId => Configuration["OpenId:ClientId"];
		private string ClientSecret => Configuration["OpenId:ClientSecret"]; // for code flow
		private string Authority => Configuration["OpenId:Instance"] + Configuration["OpenId:TenantId"];
		private string Resource => Configuration["OpenId:Resource"];
		private string CallbackPath => Configuration["OpenId:CallbackPath"];

		private string WtRealm => Configuration["WsFed:WtRealm"];
		private string MetadataAddress => Configuration["WsFed:MetadataAddress"];

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			if (Configuration["AuthenticationType"].Equals("WsFed"))
			{
				services.AddAuthentication(sharedOptions =>
				{
					sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
				})
				.AddWsFederation(options =>
				{
					options.Wtrealm = WtRealm;
					options.MetadataAddress = MetadataAddress;
					options.RefreshOnIssuerKeyNotFound = true;

					options.Events = new WsFederationEvents
					{
						OnRedirectToIdentityProvider = notifications =>
						{
							Log.Information("WsFed : OnRedirectToIdentityProvider");
							return Task.CompletedTask;
						},
						OnMessageReceived = context =>
						{
							Log.Information("WsFed : OnMessageReceived");
							return Task.CompletedTask;
						},
						OnSecurityTokenReceived = context =>
						{
							Log.Information("WsFed : OnSecurityTokenReceived");
							return Task.CompletedTask;
						},
						OnSecurityTokenValidated = context =>
						{
							Log.Information("WsFed : OnSecurityTokenValidated");
							return Task.CompletedTask;
						},
						OnTicketReceived = async context =>
						{
							Log.Information("WsFed : OnTicketReceived");

							var request = context.HttpContext.Request;
							var currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);

							var credential = new ClientCredential(ClientId, ClientSecret);
							var authContext = new AuthenticationContext(Authority);

							var result = await authContext.AcquireTokenAsync(Resource, credential);
							Log.Information("WsFed : AccessToken = ", result.AccessToken);

							// Claims
							var emailClaim = string.Empty;
							var firstTry = context.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
							if (firstTry != null && !string.IsNullOrEmpty(firstTry.Value))
							{
								emailClaim = firstTry.Value;
							}
							else
							{
								var secondTry = context.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Upn);
								if (secondTry != null && !string.IsNullOrEmpty(secondTry.Value))
								{
									emailClaim = secondTry.Value;
								}
								else
								{
									var thirdTry = context.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
									emailClaim = thirdTry.Value;
								}
							}

							//return Task.CompletedTask;
						},
					};
				})
				.AddCookie();
			}
			else if (Configuration["AuthenticationType"].Equals("OpenId"))
			{
				services.AddAuthentication(sharedOptions =>
				{
					sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
				})
				.AddOpenIdConnect(options =>
				{
					options.ClientId = ClientId;
					options.ClientSecret = ClientSecret; // for code flow
					options.Authority = Authority;
					options.CallbackPath = CallbackPath;

					options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

					options.Events = new OpenIdConnectEvents()
					{
						OnRedirectToIdentityProvider = c =>
						{
							Log.Information("OnRedirectToIdentityProvider");
							return Task.FromResult(0);
						},
						OnMessageReceived = c =>
						{
							Log.Information("OnMessageReceived");
							return Task.FromResult(0);
						},
						OnTokenValidated = c =>
						{
							Log.Information("OnTokenValidated");
							return Task.FromResult(0);
						},
						OnAuthorizationCodeReceived = async context =>
						{
							Log.Information("OnAuthorizationCodeReceived");

							var request = context.HttpContext.Request;
							var currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);

							var credential = new ClientCredential(ClientId, ClientSecret);
							var authContext = new AuthenticationContext(Authority);

							var result = await authContext.AcquireTokenByAuthorizationCodeAsync(
								context.ProtocolMessage.Code, new Uri(currentUri), credential, Resource);

							context.HandleCodeRedemption(result.AccessToken, result.IdToken);
						},
						OnTokenResponseReceived = c =>
						{
							Log.Information("OnTokenResponseReceived");
							return Task.FromResult(0);
						},
						OnTicketReceived = context =>
						{
							Log.Information("OnTicketReceived");

							// Claims
							var emailClaim = string.Empty;
							var firstTry = context.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
							if (firstTry != null && !string.IsNullOrEmpty(firstTry.Value))
							{
								emailClaim = firstTry.Value;
							}
							else
							{
								var secondTry = context.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Upn);
								if (secondTry != null && !string.IsNullOrEmpty(secondTry.Value))
								{
									emailClaim = secondTry.Value;
								}
								else
								{
									var thirdTry = context.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
									emailClaim = thirdTry.Value;
								}
							}

							return Task.FromResult(0);
						},

						OnRemoteFailure = c =>
						{
							Log.Information("OnRemoteFailure");
							return Task.FromResult(0);
						},
						OnUserInformationReceived = c =>
						{
							Log.Information("OnUserInformationReceived");
							return Task.FromResult(0);
						},
						OnRemoteSignOut = c =>
						{
							Log.Information("OnRemoteSignOut");
							return Task.FromResult(0);
						},
						OnAuthenticationFailed = c =>
						{
							Log.Information("OnAuthenticationFailed");
							return Task.FromResult(0);
						},
					};
				})
				.AddCookie();

				services.AddMvc();
			}

			services.AddMvc(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			})
			.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseAuthentication();
			app.UseSpaAuthentication();

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseCookiePolicy();

			app.UseSpaStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});

			app.UseSpa(spa =>
			{
				// To learn more about options for serving an Angular SPA from ASP.NET Core,
				// see https://go.microsoft.com/fwlink/?linkid=864501

				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseAngularCliServer(npmScript: "start");
				}
			});
		}
	}
}
