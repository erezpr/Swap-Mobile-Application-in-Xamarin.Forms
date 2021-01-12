using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Swap.Chat_Database;
using Swap.Models;
using Swap.Services;
using Swap.ViewModels;
using Swap.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Swap
{
    public partial class App : Application, INotifyPropertyChanged
    {
        private const string m_TokenKey = "token";
        private const string m_UserIdKey = "userId";
        private const string m_EmailKey = "email";
        private const string m_IsFacebookUserKey = "isFacebookUser";
        private const string m_UserNameKey = "userName";
        private const string m_PhoneNumberKey = "phoneNumber";
        private const string m_CityKey = "city";
        private const string m_NeighborhoodKey = "neighborhood";
        private const string m_FireBaseTokenKey = "firebaseToken";
        private const string m_IsUserHaveNewNotificationKey = "isUserHaveNewNotification";
        private const string m_ReceivedNotificationListKey = "receivedNotificationList";
        private const string m_SentNotificationListKey = "sentNotificationList";
        private const string m_ProfileImageBytesKey = "profileImageBytes";
        private const string m_RefreshRequiredKey = "refreshRequired";
        private const string m_DataBaseKey = "DataBase";
        private const string m_CacheDataBaseKey = "CacheDataBase";
        public NavigationPage ProfilePage { get; set; }
        public ItemViewModel ItemViewModel { get; set; }
        public ProfileViewModel ProfileViewModel { get; set; }
        public Page CurrentPage { get; set; }

        public App()
        {
            InitializeComponent();
            var current = Connectivity.NetworkAccess;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            CurrentPage = new AppShell();
            if (current == NetworkAccess.Internet)
            {
                MainPage = CurrentPage;
            }
            else
            {
                MainPage = new NoInternetPage();
            }

            Debug.WriteLine(typeof(string).Assembly.ImageRuntimeVersion);
        }
        public void SetMainPage()
        {
            MainPage = CurrentPage;
        }


        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                SetMainPage();
            }
            else
            {
                MainPage = new NoInternetPage();
            }
        }

        protected override async void OnStart()
        {
            try
            {
                IsUserHaveNewNotificationMessage = true;

                if (DataBase == null)
                {
                    DataBase = new Database();
                }
                if (MemoryCache == null)
                {
                    MemoryCache = new MemoryCache(new MemoryCacheOptions());

                }
                // Handle when your app starts
                if (string.IsNullOrWhiteSpace(Token) == true)
                    return;
                else
                {
                    RefreshRequired = true;
                    IEnumerable<InstantMessage> messages = await ServerFacade.Users.GetMessages();
                    saveMessages(messages);
                    if (HubConnection == null)
                    {
                        HubConnection = new HubConnectionBuilder()
                        .WithUrl($"http://Vmedu184.mtacloud.co.il/chatHub")
                        .Build();
                        await HubConnection.StartAsync();
                        await HubConnection.InvokeAsync("Connect", UserId);

                    }
                    ReceiveMessage();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void ReceiveMessage()
        {
            HubConnection.On<string, int, string, string, string, int>("ReceiveMessage", (chatId, toId, toUserName, fromUserName, body, fromUserid) =>
            {
                saveMessages(chatId, toId, toUserName, fromUserName, body, fromUserid);
            });
        }

        private void saveMessages(IEnumerable<InstantMessage> messages)
        {
            if (messages == null)
            {
                return;
            }
            foreach (InstantMessage message in messages)
            {
                saveMessages(message.ChatId.ToString(), UserId, UserName, message.UserName, message.Body, message.UserId);
            }

        }

        private void saveMessages(string chatId, int toId, string toUserName, string fromUserName, string body, int fromUserid)
        {
            if (!MemoryCache.TryGetValue(chatId, out Chat chat))
            {
                if (!DataBase.UserToGroupTable.TryGetChatId(fromUserid, toId, out string chatGuidId))
                {
                    UserToGroup utg1 = new UserToGroup(fromUserid, fromUserName); // insert real value
                    UserToGroup utg2 = new UserToGroup(toId, toUserName); // insert real value
                    chat = new Chat(new List<UserToGroup> { utg1, utg2 });
                    DataBase.ChatsTable.Add(chat);
                }
                else
                {
                    chat = DataBase.ChatsTable.Get(chatGuidId);
                }

                MemoryCache.Set(chatId, chat);
            }
            InstantMessage message = new InstantMessage { Body = body, UserId = fromUserid, UserName = fromUserName, Chat = chat };
            DataBase.InstantMessageTable.Add(message);
        }

        public Database DataBase
        {
            get
            {
                if (Properties.ContainsKey(m_DataBaseKey) && Properties[m_DataBaseKey] != null)
                {
                    return Properties[m_DataBaseKey] as Database;
                }

                return null;
            }
            set
            {
                Properties[m_DataBaseKey] = value;
            }
        }

        public MemoryCache MemoryCache
        {
            get
            {
                if (Properties.ContainsKey(m_CacheDataBaseKey) && Properties[m_CacheDataBaseKey] != null)
                {
                    return Properties[m_CacheDataBaseKey] as MemoryCache;
                }

                return null;
            }

            set
            {
                Properties[m_CacheDataBaseKey] = value;
            }
        }

        public ObservableCollection<NotificationItem> ReceivedNotificationList
        {
            get
            {
                if (Properties.ContainsKey(m_ReceivedNotificationListKey) && Properties[m_ReceivedNotificationListKey] != null)
                {
                    return (ObservableCollection<NotificationItem>)Properties[m_ReceivedNotificationListKey];
                }

                return null;
            }
            set
            {
                Properties[m_ReceivedNotificationListKey] = value;
            }
        }

        public ObservableCollection<NotificationItem> sentNotificationList
        {
            get
            {
                if (Properties.ContainsKey(m_SentNotificationListKey) && Properties[m_SentNotificationListKey] != null)
                {
                    return (ObservableCollection<NotificationItem>)Properties[m_SentNotificationListKey];
                }

                return null;
            }
            set
            {
                Properties[m_SentNotificationListKey] = value;
            }
        }

        public bool RefreshRequired
        {
            get
            {
                if (Preferences.ContainsKey(m_RefreshRequiredKey))
                {
                    return Preferences.Get(m_RefreshRequiredKey, false);
                }

                return false;
            }
            set
            {
                Preferences.Set(m_RefreshRequiredKey, value);
            }
        }

        public bool IsFacebookUser
        {
            get
            {
                if (Preferences.ContainsKey(m_IsFacebookUserKey))
                {
                    return Preferences.Get(m_IsFacebookUserKey, false);
                }

                return false;
            }
            set
            {
                Properties[m_IsFacebookUserKey] = value;
                Preferences.Set(m_IsFacebookUserKey, value);
            }
        }

        public bool IsUserHaveNewNotificationMessage
        {
            get
            {
                if (Preferences.ContainsKey(m_IsUserHaveNewNotificationKey))
                {
                    return Preferences.Get(m_IsUserHaveNewNotificationKey, false);
                }

                return false;
            }
            set
            {
                Preferences.Set(m_IsUserHaveNewNotificationKey, value);
            }
        }

        public int UserId
        {
            get
            {
                if (Preferences.ContainsKey(m_UserIdKey))
                {
                    return Preferences.Get(m_UserIdKey, 0);
                }

                return 0;
            }
            set
            {
                Preferences.Set(m_UserIdKey, value);
            }
        }

        public string Token
        {
            get
            {
                if (Preferences.ContainsKey(m_TokenKey))
                {
                    return Preferences.Get(m_TokenKey, "");
                }

                return "";
            }
            set
            {
                Preferences.Set(m_TokenKey, value);
            }

        }

        public string Email
        {
            get
            {
                if (Preferences.ContainsKey(m_EmailKey))
                {
                    return Preferences.Get(m_EmailKey, "");
                }

                return "";
            }
            set
            {
                Preferences.Set(m_EmailKey, value);
            }
        }

        public string UserName
        {
            get
            {
                if (Preferences.ContainsKey(m_UserNameKey))
                {
                    return Preferences.Get(m_UserNameKey, "");
                }

                return "אורח";
            }
            set
            {
                Preferences.Set(m_UserNameKey, value);
                OnPropertyChanged("UserName");
            }
        }

        public string PhoneNumber
        {
            get
            {
                if (Preferences.ContainsKey(m_PhoneNumberKey))
                {
                    return Preferences.Get(m_PhoneNumberKey, "");
                }

                return "";
            }
            set
            {
                Preferences.Set(m_PhoneNumberKey, value);
            }
        }

        public string City
        {
            get
            {
                if (Preferences.ContainsKey(m_CityKey))
                {
                    return Preferences.Get(m_CityKey, "");
                }

                return "";
            }
            set
            {
                Preferences.Set(m_CityKey, value);
            }
        }

        public string Neighborhood
        {
            get
            {
                if (Preferences.ContainsKey(m_NeighborhoodKey))
                {
                    return Preferences.Get(m_NeighborhoodKey, "");
                }

                return "";
            }
            set
            {
                Preferences.Set(m_NeighborhoodKey, value);
            }
        }

        public string FireBaseToken
        {
            get
            {
                if (Preferences.ContainsKey(m_FireBaseTokenKey))
                {
                    return Preferences.Get(m_FireBaseTokenKey, "");
                }

                return "";
            }
            set
            {
                Preferences.Set(m_FireBaseTokenKey, value);
            }
        }

        public HubConnection HubConnection { get; set; }

        public string ProfileImageBytes
        {
            get
            {
                if (Properties.ContainsKey(m_ProfileImageBytesKey) && Properties[m_ProfileImageBytesKey] != null)
                {
                    return Properties[m_ProfileImageBytesKey].ToString();
                }

                return "";
            }
            set
            {
                Properties[m_ProfileImageBytesKey] = value;
            }
        }

        public void UpdateUserDetails(int i_UserId, string i_Token, string i_Email, string i_UserName, string i_City, string i_Neighborhood, string i_PhoneNumber)
        {
            UserId = i_UserId;
            Token = i_Token;
            Email = i_Email;
            UserName = i_UserName;
            PhoneNumber = i_PhoneNumber;
            City = i_City;
            Neighborhood = i_Neighborhood;
        }

        internal async void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            const string Url = "http://Vmedu184.mtacloud.co.il/user/updatecoordinate";
            HttpClient client = new HttpClient();

            IGeolocator locator = CrossGeolocator.Current;
            if (locator.IsListening == false)
            {
                await locator.StartListeningAsync(new TimeSpan(0, 0, 5), 100);
            }
            locator.DesiredAccuracy = 1;

            var position = await locator.GetPositionAsync(timeout: new TimeSpan(0, 0, 5));

            UserLocationUpdates userModel = new UserLocationUpdates
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Id = UserId
            };

            string json = JsonConvert.SerializeObject(userModel);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            StringContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("Application/json");
            await client.PostAsync(Url, content);
        }
    }
}