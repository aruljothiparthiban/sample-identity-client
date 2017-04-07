using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security.Cookies;
using System.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;
using IdentityModel.Client;
using IdentityServer3.Core;
using System.Web.Helpers;

[assembly: OwinStartup(typeof(SampleClient.Startup))]

namespace SampleClient
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            OpenIdConnectAuthenticationOptions options = new OpenIdConnectAuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["openid:ClientId"],
                Authority = ConfigurationManager.AppSettings["openid:Authority"],
                RedirectUri = ConfigurationManager.AppSettings["openid:RedirectUri"],
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["openid:PostLogoutRedirectUri"],
                ResponseType = ConfigurationManager.AppSettings["openid:ResponseType"],
                Scope = ConfigurationManager.AppSettings["openid:Scope"],
                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies",
                Notifications = GetNotificationConfig()
            };

            app.UseOpenIdConnectAuthentication(options);

        }

        private OpenIdConnectAuthenticationNotifications GetNotificationConfig()
        {
            return new OpenIdConnectAuthenticationNotifications
            {
                SecurityTokenValidated = async n =>
                {
                    var nid = new ClaimsIdentity(
                        n.AuthenticationTicket.Identity.AuthenticationType,
                        Constants.ClaimTypes.GivenName,
                        Constants.ClaimTypes.Role);

                    // get userinfo data
                    var userInfoClient = new UserInfoClient(
                        new Uri(n.Options.Authority + "/connect/userinfo"),
                        n.ProtocolMessage.AccessToken);

                    var userInfo = await userInfoClient.GetAsync();
                    userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

                    // keep the id_token for logout
                    nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                    // add access token for sample API
                    nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                    // keep track of access token expiration
                    nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                    // add some other app specific claim
                    nid.AddClaim(new Claim("app_specific", "some data"));

                    n.AuthenticationTicket = new AuthenticationTicket(
                        nid,
                        n.AuthenticationTicket.Properties);
                },

                RedirectToIdentityProvider = n =>
                {
                    if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                    {
                        var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                        if (idTokenHint != null)
                        {
                            n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                        }
                    }

                    return Task.FromResult(0);
                },

                AuthenticationFailed = context => {
                    string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                    context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                    context.HandleResponse();
                    context.Response.Redirect(context.ProtocolMessage.RedirectUri);
                    return Task.FromResult(0);
                }
            };
        }
    }
}