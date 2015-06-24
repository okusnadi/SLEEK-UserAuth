using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Configuration;
using Thinktecture.IdentityServer.Core.Configuration;
using IdentityManager.Configuration;
using Microsoft.Owin.Security.Twitter;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using WebHost.IdMgr;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.IdentityModel.Protocols;
using Thinktecture.IdentityModel.Client;
using System.Globalization;
using System.Web.Helpers;
using WebHost.Configuration;
using Thinktecture.IdentityServer.Core.Services;

[assembly: OwinStartup(typeof(WebHost.Startup))]

namespace WebHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            //var options = new IdentityServerOptions
            //{
            //    SigningCertificate = Certificate.Load(),
            //    Factory = factory,
            //};

            //appBuilder.UseIdentityServer(options);

            var connectionString = "MembershipReboot";
                        

            //------------------------------------------------
            const string IdServBaseUri = @"https://localhost:44333"; ///core
            const string ClientUri = @"https://localhost:44333/admin/";

            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            appBuilder.Map("/admin", adminApp =>
            {
                adminApp.UseCookieAuthentication(new CookieAuthenticationOptions { AuthenticationType = "Cookies" });

                adminApp.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        AuthenticationType = "oidc",
                        ClientId = "hybridclient",
                        Authority = IdServBaseUri,
                        RedirectUri = ClientUri,
                        PostLogoutRedirectUri = @"https://localhost:44333/admin/", //ClientUri,                        
                        ResponseType = "code id_token token",
                        Scope = "openid profile email roles all_claims", //offline_access
                        SignInAsAuthenticationType = "Cookies",
                        Notifications = new OpenIdConnectAuthenticationNotifications
                            {
                                AuthorizationCodeReceived = async n => 
                                {
                                    var identity = n.AuthenticationTicket.Identity;

                                    var nIdentity = new ClaimsIdentity(identity.AuthenticationType, "email", "role");

                                    var userInfoClient = new UserInfoClient(
                                        new Uri(@"https://localhost:44333/connect/userinfo"),
                                        n.ProtocolMessage.AccessToken);

                                    var userInfo = await userInfoClient.GetAsync();

                                    List<Tuple<string, string>> mn = userInfo.Claims as List<Tuple<string, string>>;
                                    mn.ForEach(x => nIdentity.AddClaim(new Claim(x.Item1, x.Item2)));
                                    //userInfo.Claims.ToList().ForEach(x => nIdentity.AddClaim(new Claim(x.Item1, x.Item2)));

                                    /*var tokenClient = new OAuth2Client(new Uri(@"https://localhost:44333/connect/token"), "hybridclient", "secret");
                                    var response = await tokenClient.RequestAuthorizationCodeAsync(n.Code, n.RedirectUri);

                                    nIdentity.AddClaim(new Claim("access_token", response.AccessToken));
                                    nIdentity.AddClaim(
                                        new Claim("expires_at", DateTime.UtcNow.AddSeconds(response.ExpiresIn).ToLocalTime().ToString(CultureInfo.InvariantCulture)));
                                    nIdentity.AddClaim(new Claim("refresh_token", response.RefreshToken));*/

                                    nIdentity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                                    
                                    n.Options.Caption += "id_token_hint=" + n.ProtocolMessage.IdToken; 
                                    //n.Options.PostLogoutRedirectUri += "id_token_hint=" + n.ProtocolMessage.IdToken;                                    
                                  
                                    n.AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(
                                    nIdentity,
                                    n.AuthenticationTicket.Properties);
                                },
                                RedirectToIdentityProvider = async n =>
                                {
                                    if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                                    {
                                        var caption = n.Options.Caption;
                                        var indexOfTokenHint = caption.IndexOf("id_token_hint=");
                                        var idTokenHint = caption.Substring(indexOfTokenHint).Replace("id_token_hint=", "");
                                        n.Options.Caption = caption.Remove(indexOfTokenHint);                                        
                                        n.ProtocolMessage.IdTokenHint = idTokenHint;

                                        /*var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token").Value;
                                        n.ProtocolMessage.IdTokenHint = idTokenHint;*/
                                    }
                                }
                            }
                        
                    });

                var factory = new IdentityManagerServiceFactory();
                factory.Configure(connectionString);
                
                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory,
                    SecurityConfiguration = new HostSecurityConfiguration() { AdminRoleName = "Admin", HostAuthenticationType = "Cookies", AdditionalSignOutType = "oidc" },
                    //AdminRoleName = "IdentityManagerAdministrator", 
                });
            });
            
            //------------------------------------------------            

            appBuilder.Map("/UserAccount", userAccountApp =>
            {
                userAccountApp.UseCookieAuthentication(new CookieAuthenticationOptions() { AuthenticationType = "Cookies" });

                userAccountApp.UseOpenIdConnectAuthentication(
                        new OpenIdConnectAuthenticationOptions
                        {
                            AuthenticationType = "oidc",
                            ClientId = "hybridclient",
                            Authority = IdServBaseUri,
                            RedirectUri = @"https://localhost:44333/useraccount/",
                            ResponseType = "code id_token token",
                            Scope = "openid profile email roles all_claims", //offline_access
                            SignInAsAuthenticationType = "Cookies",
                            PostLogoutRedirectUri = @"https://localhost:44333/permissions/",
                            Notifications = new OpenIdConnectAuthenticationNotifications
                            {
                                AuthorizationCodeReceived = async n =>
                                {
                                    var identity = n.AuthenticationTicket.Identity;

                                    var nIdentity = new ClaimsIdentity(identity.AuthenticationType, "email", "role");

                                    var userInfoClient = new UserInfoClient(
                                        new Uri(@"https://localhost:44333/connect/userinfo"),
                                        n.ProtocolMessage.AccessToken);

                                    var userInfo = await userInfoClient.GetAsync();

                                    List<Tuple<string, string>> mn = userInfo.Claims as List<Tuple<string, string>>;
                                    mn.ForEach(x => nIdentity.AddClaim(new Claim(x.Item1, x.Item2)));
                                   
                                    nIdentity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                                    n.Options.Caption += "id_token_hint=" + n.ProtocolMessage.IdToken;                                  

                                    n.AuthenticationTicket = new Microsoft.Owin.Security.AuthenticationTicket(
                                    nIdentity,
                                    n.AuthenticationTicket.Properties);
                                },
                                RedirectToIdentityProvider = async n =>
                                {
                                    if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                                    {
                                        var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token").Value;
                                        n.ProtocolMessage.IdTokenHint = idTokenHint;
                                        //var caption = n.Options.Caption;
                                        //var indexOfTokenHint = caption.IndexOf("id_token_hint=");
                                        //var idTokenHint = caption.Substring(indexOfTokenHint).Replace("id_token_hint=", "");
                                        //n.Options.Caption = caption.Remove(indexOfTokenHint);
                                        //n.ProtocolMessage.IdTokenHint = idTokenHint;
                                    }
                                }
                            }
                        });                

            });            

            var idSvrFactory = Factory.Configure();
            idSvrFactory.ConfigureCustomUserService(connectionString);
            idSvrFactory.ViewService = new Thinktecture.IdentityServer.Core.Configuration.Registration<IViewService>(typeof(CustomViewService));
            
            var options = new IdentityServerOptions
            {
                IssuerUri = "https://localhost:44333/",  //"https://localhost:44333/core"
                SiteName = "SLEEK Auth System",
                
                SigningCertificate = Certificate.Load(),  //.Get(),
                Factory = idSvrFactory,
                CorsPolicy = CorsPolicy.AllowAll,
                AuthenticationOptions = new AuthenticationOptions
                {
                    IdentityProviders = ConfigureAdditionalIdentityProviders,                    
                    LoginPageLinks = new LoginPageLink[]
                    {
                        new LoginPageLink(){
                            Href = "UserAccount/PasswordReset",
                            Text = "I can't access my account"
                        },
                        new LoginPageLink(){
                            Href = "Registration",
                            Text = "Create account"
                        }
                    }
                },
                //EnableWelcomePage = false
            };

            appBuilder.UseIdentityServer(options);
                       

        }

        public static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "client", //"767400843187-8boio83mb57ruogr9af9ut09fkg56b27.apps.googleusercontent.com",
                ClientSecret = "secret"  //"5fWcBT0udKY7_b6E3gEiJlze"
            };
            app.UseGoogleAuthentication(google);

            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "app", //"676607329068058",
                AppSecret = "secret"  //"9d6ab75f921942e61fb43a9b1fc25c63"
            };
            app.UseFacebookAuthentication(fb);

            var twitter = new TwitterAuthenticationOptions
            {
                AuthenticationType = "Twitter",
                SignInAsAuthenticationType = signInAsType,
                ConsumerKey = "consumer",  //"N8r8w7PIepwtZZwtH066kMlmq",
                ConsumerSecret = "secret"  //"df15L2x6kNI50E4PYcHS0ImBQlcGIt6huET8gQN41VFpUCwNjM"
            };
            app.UseTwitterAuthentication(twitter);
        }
    }
}