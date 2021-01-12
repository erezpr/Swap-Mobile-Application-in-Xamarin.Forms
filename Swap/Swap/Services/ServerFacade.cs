using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Swap.Chat_Database;
using Swap.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;

namespace Swap.Services
{
    static class ServerFacade
    {
        static public class Users
        {
            public static async Task<SignUpUserResult> SignUpAsync(SignUpUser i_SignUpUser)
            {
                IGeolocator locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 1;

                if (locator.IsListening == false)
                {
                    await locator.StartListeningAsync(new TimeSpan(0, 0, 5), 100);
                    Position position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5));
                    locator.PositionChanged += (Application.Current as App).Locator_PositionChanged;
                    i_SignUpUser.Longitude = position.Longitude;
                    i_SignUpUser.Latitude = position.Latitude;
                }

                i_SignUpUser.FirebaseToken = (Application.Current as App).FireBaseToken;

                string responseContent = await postAsync("user/signup", i_SignUpUser);
                SignUpUserResult signUpUser = JsonConvert.DeserializeObject<SignUpUserResult>(responseContent);
                return signUpUser;
            }

            public static async Task<LoginUserResult> LoginAsync(LoginUser i_LoginUser)
            {
                string responseContent = await postAsync("user/login", i_LoginUser);

                return JsonConvert.DeserializeObject<LoginUserResult>(responseContent); ;
            }

            public static async Task UpdateUserAsync(UpdateUser i_UpdateUser)
            {
                await postAsync("user/UpdateProfile", i_UpdateUser, true);
            }

            public static async Task<LoginUserResult> GetUserInfoAsync(int i_UserId)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpMethod httpMethod = HttpMethod.Get;
                    string userId = string.Format("id={0}", i_UserId);
                    string token = "Bearer " + (Application.Current as App).Token;
                    string uri = "http://Vmedu184.mtacloud.co.il/user/getUser" + "?" + userId;
                    HttpRequestMessage request = new HttpRequestMessage() { Method = httpMethod };
                    request.RequestUri = new Uri(uri);
                    request.Headers.Add("Authorization", token);
                    HttpResponseMessage httpResponse = await client.SendAsync(request);
                    httpResponse.EnsureSuccessStatusCode();
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    LoginUserResult a = JsonConvert.DeserializeObject<LoginUserResult>(responseContent);
                    return a;
                }
            }

            private static async Task<string> postAsync(string i_Path, object i_Content, bool i_IsAuth = false)
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(i_Content);
                    HttpContent content = new StringContent(json);
                    if (i_IsAuth == true)
                    {
                        string token = (Application.Current as App).Token;
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        content.Headers.ContentType = new MediaTypeHeaderValue("Application/json");
                    }

                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage httpResponse = await client.PostAsync("http://Vmedu184.mtacloud.co.il/" + i_Path, content);
                    httpResponse.EnsureSuccessStatusCode();

                    return await httpResponse.Content.ReadAsStringAsync();
                }
            }

            public static async Task<IEnumerable<InstantMessage>> GetMessages()
            {
                Database database = (Application.Current as App).DataBase;
                int userId = (Application.Current as App).UserId;
                InstantMessage lastMessage = database.InstantMessageTable.GetLast();
                if (null == lastMessage)
                    return null;

                string timeStamp = lastMessage.Time.Ticks.ToString();
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"http://Vmedu184.mtacloud.co.il/user/getMessages?id={userId}&timeStamp={timeStamp}")
                };

                request.Headers.Add("Authorization", "Bearer " + (Application.Current as App).Token);
                using (HttpClient http = new HttpClient())
                {
                    HttpResponseMessage response = await http.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        await Task.CompletedTask;
                    string content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<InstantMessage>>(content);
                }
            }
        }

        static public class Items
        {
            public static async Task<List<Item>> SearchItemsAsync(string i_Parameters)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpMethod httpMethod = HttpMethod.Get;
                    string Token = "Bearer " + (Application.Current as App).Token;

                    string Url = "http://Vmedu184.mtacloud.co.il/item/filter";

                    if (i_Parameters != null)
                    {
                        Url += "?";
                        Url += i_Parameters;
                    }

                    var request = new HttpRequestMessage()
                    {
                        Method = httpMethod,
                        RequestUri = new Uri(Url)
                    };

                    request.Headers.Add("Authorization", Token);

                    HttpResponseMessage httpResponse = await client.SendAsync(request);
                    httpResponse.EnsureSuccessStatusCode();
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<Item>>(responseContent);
                }
            }

            public static async Task DeleteItem(Item i_Item)
            {
                using (HttpClient client = new HttpClient())
                {
                    const string Url = "http://Vmedu184.mtacloud.co.il/item/delete";
                    string token = (Application.Current as App).Token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(i_Item);
                    StringContent content = new StringContent(json);
                    content.Headers.ContentType = new MediaTypeHeaderValue("Application/json");
                    HttpResponseMessage httpResponse = await client.PostAsync(Url, content);
                    httpResponse.EnsureSuccessStatusCode();
                }
            }

            public static async Task EditItem(Item i_Item)
            {
                using (HttpClient client = new HttpClient())
                {
                    const string Url = "http://Vmedu184.mtacloud.co.il/item/update";
                    string token = (Application.Current as App).Token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(i_Item);
                    StringContent content = new StringContent(json);
                    content.Headers.ContentType = new MediaTypeHeaderValue("Application/json");
                    HttpResponseMessage httpResponse = await client.PostAsync(Url, content);
                    httpResponse.EnsureSuccessStatusCode();
                }
            }

            public static async Task AddItem(Item i_Item)
            {
                using (HttpClient client = new HttpClient())
                {
                    const string Url = "http://Vmedu184.mtacloud.co.il/item/create";
                    string token = (Application.Current as App).Token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(i_Item);
                    StringContent content = new StringContent(json);
                    content.Headers.ContentType = new MediaTypeHeaderValue("Application/json");
                    HttpResponseMessage httpResponse = await client.PostAsync(Url, content);
                    httpResponse.EnsureSuccessStatusCode();
                }
            }

            public static async Task IncrementItemViewsNumber(int i_ItemId)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpMethod httpMethod = HttpMethod.Get;
                    string data = string.Format("id={0}", i_ItemId);
                    string UrlAllItems = "http://Vmedu184.mtacloud.co.il/item/incview" + "?" + data;
                    HttpRequestMessage request = new HttpRequestMessage() { Method = httpMethod };
                    request.RequestUri = new Uri(UrlAllItems);
                    HttpResponseMessage httpResponse = await client.SendAsync(request);
                    httpResponse.EnsureSuccessStatusCode();
                }
            }

            public static async Task<List<Item>> GetMostViewedItems(int i_ItemType, int i_Count)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpMethod httpMethod = HttpMethod.Get;
                    string UrlAllItems = string.Format("http://Vmedu184.mtacloud.co.il/item/getmostviewed?ty={0}&lim={1}", (int)i_ItemType, i_Count);
                    HttpRequestMessage request = new HttpRequestMessage() { Method = httpMethod };
                    request.RequestUri = new Uri(UrlAllItems);
                    HttpResponseMessage httpResponse = await client.SendAsync(request);
                    httpResponse.EnsureSuccessStatusCode();
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<Item>>(responseContent);
                }
            }

            public static async Task<Item> GetItemInfoAsync(int i_ItemId)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpMethod httpMethod = HttpMethod.Get;
                    string userId = string.Format("id={0}", i_ItemId);
                    string token = "Bearer " + (Application.Current as App).Token;
                    string uri = "http://Vmedu184.mtacloud.co.il/item/getItem" + "?" + userId;
                    HttpRequestMessage request = new HttpRequestMessage() { Method = httpMethod };
                    request.RequestUri = new Uri(uri);
                    request.Headers.Add("Authorization", token);
                    HttpResponseMessage httpResponse = await client.SendAsync(request);
                    httpResponse.EnsureSuccessStatusCode();
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<Item>(responseContent);
                }
            }
        }

        public static class Trades
        {
            public static async Task OfferTradeAsync(Trade trade)
            {
                using (HttpClient client = new HttpClient())
                {
                    const string Url = "http://Vmedu184.mtacloud.co.il/trade/offertrade";
                    string token = (Application.Current as App).Token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(trade);
                    StringContent content = new StringContent(json);
                    content.Headers.ContentType = new MediaTypeHeaderValue("Application/json");
                    HttpResponseMessage httpResponse = await client.PostAsync(Url, content);
                    httpResponse.EnsureSuccessStatusCode();
                }
            }

            public static async Task AnswerTradeAsync(Trade trade)
            {
                using (HttpClient client = new HttpClient())
                {
                    const string Url = "http://Vmedu184.mtacloud.co.il/trade/answertrade";
                    string token = (Application.Current as App).Token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string json = JsonConvert.SerializeObject(trade);
                    StringContent content = new StringContent(json);
                    content.Headers.ContentType = new MediaTypeHeaderValue("Application/json");
                    HttpResponseMessage httpResponse = await client.PostAsync(Url, content);
                    httpResponse.EnsureSuccessStatusCode();
                }
            }

            public static async Task<List<Trade>> GetNotificationListAsync(int i_UserId)
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpMethod httpMethod = HttpMethod.Get;
                    string token = (Application.Current as App).Token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string data = string.Format("id={0}", i_UserId);
                    string UrlAllItems = "http://Vmedu184.mtacloud.co.il/trade/gettradenotifications" + "?" + data;
                    HttpRequestMessage request = new HttpRequestMessage() { Method = httpMethod };
                    request.RequestUri = new Uri(UrlAllItems);
                    HttpResponseMessage httpResponse = await client.SendAsync(request);
                    httpResponse.EnsureSuccessStatusCode();
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<Trade>>(responseContent);
                }
            }
        }
    }
}