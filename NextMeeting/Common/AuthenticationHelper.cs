using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;

namespace NextMeeting.Common
{
    public class AuthenticationHelper : IAuthenticationProvider
    {

        static readonly string AuthorityFormat = App.Current.Resources["ida:AADInstance"].ToString() + "{0}/";
        static readonly string SignoutUrlFormat = App.Current.Resources["ida:AADInstance"].ToString() + "/{0}/oauth2/logout";
        static readonly string ClientId = App.Current.Resources["ida:ClientId"].ToString();
        static readonly string LastTenantId = App.Current.Resources["ida:Domain"].ToString();

        // To authenticate to Microsoft Graph, the client needs to know its App ID URI.
        public const string GraphResourceId = "https://graph.microsoft.com/";
        public const string GraphEndpointId = "https://graph.microsoft.com/v1.0/";
   
        private static AuthenticationContext authContext = null;

        private static AuthenticationHelper current;
        public static AuthenticationHelper Current
        {
            get
            {
                return (current == null ? current = new AuthenticationHelper() : current);
            }
        }
        public static Uri AppRedirectURI
        {
            get
            {
                return new Uri(string.Format("ms-appx-web://microsoft.aad.brokerplugin/{0}", WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper()));
            }
        }
        public static string Authority
        {
            get
            {
                return String.Format(AuthorityFormat, LastTenantId);
            }
        }
        public static Uri LogOutUri
        {
            get
            {
                return new Uri(String.Format(SignoutUrlFormat, LastTenantId));
            }
        }
        public async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            if (authContext == null)
                authContext = new AuthenticationContext(Authority);

            AuthenticationResult authResult = null;

            authResult = await authContext.AcquireTokenSilentAsync(GraphResourceId, ClientId);

            if (authResult.Status != AuthenticationStatus.Success && authResult.Error == "failed_to_acquire_token_silently")
                authResult = await authContext.AcquireTokenAsync(GraphResourceId, ClientId, AppRedirectURI, PromptBehavior.Auto);

            if (authResult.Status != AuthenticationStatus.Success)
            {
                if (authResult.Error == "authentication_canceled")
                {
                    // The user cancelled the sign-in, no need to display a message.
                }
                else
                {
                    MessageDialog dialog = new MessageDialog(string.Format("If the error continues, please contact your administrator.\n\nError: {0}\n\n Error Description:\n\n{1}", authResult.Error, authResult.ErrorDescription), "Sorry, an error occurred while signing you in.");
                    await dialog.ShowAsync();
                }
            }


            return authResult;

        }
        public async Task SignOutAsync()
        {
            authContext = new AuthenticationContext(Authority);
            authContext.TokenCache.Clear();
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, LogOutUri);
            var r = await client.SendAsync(request);

        }
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var authResult = await GetAccessTokenAsync();

            if (authResult.Status != AuthenticationStatus.Success)
            {
                throw new ServiceException(new Error
                {
                     Code = authResult.Error,
                     Message = authResult.ErrorDescription
                });
            }

            request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken); // + token
        }
    }
}
