using Newtonsoft.Json;
using Swap.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Swap.Services
{
    public class FaceBookLoginServices
    {
        private readonly string r_ClientID = "891791181020028";
        private readonly float r_Version = 3.3f;

        public string GetFacebookLoginUrl()
        {
            return string.Format(
                            "https://www.facebook.com/v{0}/dialog/oauth?" +
                            "response_type=token" +
                            "&client_id={1}" +
                            "&redirect_uri=https://www.facebook.com/connect/login_success.html"+
                            "&auth_type=rerequest"+
                            "&scope=email" 
                            , r_Version, r_ClientID);
        }
        
        public async Task<FacebookUserProfile> GetUserProfile(string i_Url)
        {
            string accessToken = extractAccessTokenFromURL(i_Url);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var userProfile = await getFacebookProfileAsync(accessToken);
                return userProfile;
            }

            return null;
        }

        private string extractAccessTokenFromURL(string i_Url)
        {
            string accessToken = string.Empty;

            if (i_Url.Contains("access_token"))
            {
                accessToken = i_Url.Replace("https://www.facebook.com/connect/login_success.html#access_token=", "");
                accessToken = accessToken.Remove(accessToken.IndexOf("&"));
            }

            return accessToken;
        }

        private async Task<FacebookUserProfile> getFacebookProfileAsync(string i_AccessToken)
        {
            string requestUrl = string.Format(
                "https://graph.facebook.com/v{0}/me?" +
                "access_token={1}" +
                "&fields=id,name,email,picture.type(large){{url}}"
                , r_Version, i_AccessToken);

            using (HttpClient httpClient = new HttpClient())
            {
                string userJson = await httpClient.GetStringAsync(requestUrl);

                FacebookUserProfile userProfile = JsonConvert.DeserializeObject<FacebookUserProfile>(userJson);

                return userProfile;
            }
        }
    }
}