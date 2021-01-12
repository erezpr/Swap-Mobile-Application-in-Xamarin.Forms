using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Swap.Chat_Database;
using Swap.Enums;
using Swap.Models;
using Swap.Services;
using Swap.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Swap.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly FaceBookLoginServices r_FacebookLoginServices = new FaceBookLoginServices();

        public event Action ModeChanged;

        private ObservableCollection<string> m_Cities;
        public ObservableCollection<string> Cities
        {
            get { return m_Cities; }
            set { SetValue(ref m_Cities, value); }
        }

        private RegisterMode m_Mode;
        public RegisterMode Mode
        {
            get { return m_Mode; }
            set
            {
                SetValue(ref m_Mode, value);
                ModeChanged?.Invoke();
            }
        }

        private bool m_IsUserLogin;
        public bool IsUserLogin
        {
            get { return m_IsUserLogin; }
            set { SetValue(ref m_IsUserLogin, value); }
        }

        private bool m_IsFBSignUpMode;
        public bool IsFBSignUpMode
        {
            get { return m_IsFBSignUpMode; }
            set { SetValue(ref m_IsFBSignUpMode, value); }
        }

        private string m_Massage;
        public string Massage
        {
            get { return m_Massage; }
            set { SetValue(ref m_Massage, value); }
        }

        private string m_Email;
        public string Email
        {
            get { return m_Email; }
            set { SetValue(ref m_Email, value); }
        }

        private string m_Password;
        public string Password
        {
            get { return m_Password; }
            set { SetValue(ref m_Password, value); }
        }

        private string m_ConfirmPassword;
        public string ConfirmPassword
        {
            get { return m_ConfirmPassword; }
            set { SetValue(ref m_ConfirmPassword, value); }
        }

        private string m_PhoneNumber;
        public string PhoneNumber
        {
            get { return m_PhoneNumber; }
            set { SetValue(ref m_PhoneNumber, value); }
        }

        private string m_City;
        public string City
        {
            get { return m_City; }
            set { SetValue(ref m_City, value); }
        }

        private string m_SelectedCity;
        public string SelectedCity
        {
            get { return m_SelectedCity; }
            set { SetValue(ref m_SelectedCity, value); }
        }

        private string m_Neighborhood;
        public string Neighborhood
        {
            get { return m_Neighborhood; }
            set { SetValue(ref m_Neighborhood, value); }
        }

        private string m_UserName;
        public string UserName
        {
            get { return m_UserName; }
            set { SetValue(ref m_UserName, value); }
        }

        private string m_SubmitButtonText;
        public string SubmitButtonText
        {
            get { return m_SubmitButtonText; }
            set { SetValue(ref m_SubmitButtonText, value); }
        }

        private string m_LabelText;
        public string LabelText
        {
            get { return m_LabelText; }
            set { SetValue(ref m_LabelText, value); }
        }

        private string m_SwitchFormButtonText;
        public string SwitchFormButtonText
        {
            get { return m_SwitchFormButtonText; }
            set { SetValue(ref m_SwitchFormButtonText, value); }
        }

        private RegisterPage m_RegisterPage;
        public RegisterPage RegisterPage
        {
            get { return m_RegisterPage; }
            set { SetValue(ref m_RegisterPage, value); }
        }

        public RegisterViewModel(RegisterPage i_Page)
        {
            Cities = new ObservableCollection<string>();
            RegisterPage = i_Page;
            Mode = RegisterMode.Loggin;
            SubmitButtonText = "התחבר";
            LabelText = "עדיין לא הצטרפת?";
            SwitchFormButtonText = "הצטרף עכשיו.";
            IsUserLogin = !string.IsNullOrWhiteSpace(app.Token);
            IsBusy = false;
            IsFBSignUpMode = false;
        }

        private async Task login(string i_Email, string i_Password)
        {
            LoginUserResult user = await ServerFacade.Users.LoginAsync(new LoginUser
            {
                Email = i_Email,
                Password = i_Password
            });

            app.UpdateUserDetails(user.Id, user.Token, user.Email, user.FirstName, user.City, user.LastName, user.CellPhone);

            HubConnection connection = (Application.Current as App).HubConnection;
            if (connection == null)
            {
                connection = new HubConnectionBuilder()
                .WithUrl($"http://Vmedu184.mtacloud.co.il/chatHub")
                .Build();
                await connection.StartAsync();
                await connection.InvokeAsync("Connect", (Application.Current as App).UserId);
                (Application.Current as App).HubConnection = connection;
            }

            app.ReceiveMessage();
            if (app.DataBase == null)
            {
                app.DataBase = new Database();
            }

            if (app.MemoryCache == null)
            {
                app.MemoryCache = new MemoryCache(new MemoryCacheOptions());
            }

            await ServerFacade.Users.UpdateUserAsync(new UpdateUser { Id = app.UserId, FirebaseToken = app.FireBaseToken });
            if (user.Images.Count != 0)
            {
                app.ProfileImageBytes = user.Images[0].BytesOfImage;
            }

            IsUserLogin = true;
            app.RefreshRequired = true;
            await Shell.Current.GoToAsync("//mainPage/home");
        }

        private ICommand m_Register;
        public ICommand Register => m_Register ?? (m_Register = new Command(async () =>
        {
            if (checkInputValidity())
            {
                Massage = "";
                switch (Mode)
                {
                    case RegisterMode.Loggin:
                        {
                            try
                            {
                                IsBusy = true;
                                await login(Email, Password);
                                app.IsFacebookUser = false;
                                IsBusy = false;

                            }
                            catch (Exception)
                            {
                                IsBusy = false;
                                await RegisterPage.DisplayAlert("שגיאה", "אנא וודא כי שם המשתמש או הסיסמה תקינים", "אישור");
                            }
                            break;
                        }
                    case RegisterMode.SignUp:
                        {
                            switchFormMode();
                            IsBusy = true;
                            try
                            {
                                bool UserWantsToOpenGpsSettings = await RegisterPage.AskUserToTurnOnGPS();

                                if (!UserWantsToOpenGpsSettings)
                                {
                                    throw new GpsNotAvailableException();
                                }
                                SignUpUserResult user = await ServerFacade.Users.SignUpAsync(new SignUpUser
                                {
                                    Email = Email,
                                    Password = Password,
                                    FirstName = UserName,
                                    CellPhone = PhoneNumber,
                                    City = City,
                                    LastName = Neighborhood
                                });

                                app.UpdateUserDetails(user.Id, user.Token, Email, UserName, City, Neighborhood, PhoneNumber);

                                HubConnection connection = (Application.Current as App).HubConnection;
                                if (connection == null)
                                {

                                    connection = new HubConnectionBuilder()
                                    .WithUrl($"http://Vmedu184.mtacloud.co.il/chatHub")
                                    .Build();
                                    await connection.StartAsync();
                                    await connection.InvokeAsync("Connect", (Application.Current as App).UserId);
                                    (Application.Current as App).HubConnection = connection;
                                }
                                app.ReceiveMessage();

                                if (app.DataBase == null)
                                {
                                    app.DataBase = new Database();
                                }

                                if (app.MemoryCache == null)
                                {
                                    app.MemoryCache = new MemoryCache(new MemoryCacheOptions());

                                }

                                IsUserLogin = true;
                                app.RefreshRequired = true;

                                if (IsFBSignUpMode == true)
                                {
                                    IsFBSignUpMode = false;
                                }
                                else
                                {
                                    app.IsFacebookUser = false;
                                }

                                await Shell.Current.GoToAsync("//mainPage/home");
                            }
                            catch (Exception e)
                            {
                                if (e is GpsNotAvailableException)
                                {
                                    await RegisterPage.DisplayAlert("שגיאה", "אנא הדלק את כפתור הGPS\n לאחר תהליך הרישום תוכל לכבות אותו בחזרה.", "אישור");
                                }
                                else
                                {
                                    await RegisterPage.DisplayAlert("שגיאה", "הינך רשום כבר במערכת", "אישור");
                                }
                            }
                            finally
                            {
                                IsBusy = false;
                            }
                            break;
                        }
                }
            }
        }));

        private ICommand m_FacebookLogin;
        public ICommand FacebookLogin => m_FacebookLogin ?? (m_FacebookLogin = new Command(async () =>
        {
            IsBusy = true;

            WebView webView = new WebView
            {
                Source = r_FacebookLoginServices.GetFacebookLoginUrl(),
                HeightRequest = 1,
            };

            webView.Navigated += webViewNavigatedAsync;

            ContentPage contentPage = new ContentPage() { Content = webView };
            NavigationPage.SetHasNavigationBar(contentPage, false);
            await Shell.Current.Navigation.PushAsync(contentPage);

            IsBusy = false;
        }));

        private ICommand m_SwitchFormModer;
        public ICommand SwitchFormMode => m_SwitchFormModer ?? (m_SwitchFormModer = new Command(switchFormMode));

        private void switchFormMode()
        {
            IsBusy = true;
            switch (Mode)
            {
                case RegisterMode.SignUp:
                    {
                        Mode = RegisterMode.Loggin;
                        Massage = "";
                        SubmitButtonText = "התחבר";
                        LabelText = "עדיין לא הצטרפת?";
                        SwitchFormButtonText = "הצטרף עכשיו.";
                        break;
                    }
                case RegisterMode.Loggin:
                    {
                        Mode = RegisterMode.SignUp;
                        Massage = "";
                        SubmitButtonText = "הצטרף";
                        LabelText = "נרשמת כבר?";
                        SwitchFormButtonText = "התחבר עכשיו.";
                        break;
                    }
            }
            IsBusy = false;
        }

        private ICommand m_Logout;
        public ICommand Logout => m_Logout ?? (m_Logout = new Command(() =>
        {
            IsBusy = true;
            app.Token = "";
            app.UserId = 0;
            app.UserName = "אורח";
            clearForm();
            Thread.Sleep(1000);
            if (app.HubConnection != null)
            {
                app.HubConnection.StopAsync();
            }

            app.HubConnection = null;
            IsUserLogin = false;
            IsBusy = false;
        }));

        private ICommand m_CitySelected;
        public ICommand CitySelected => m_CitySelected ?? (m_CitySelected = new Command(() =>
        {
            if (SelectedCity == null)
            {
                return;
            }

            if (SelectedCity.Trim() != "אין תוצאות")
            {
                City = SelectedCity.Trim();
            }

            SelectedCity = null;
        }));

        private async void webViewNavigatedAsync(object sender, WebNavigatedEventArgs e)
        {
            FacebookUserProfile FBProfile = await r_FacebookLoginServices.GetUserProfile(e.Url);

            if (FBProfile != null)
            {
                app.IsFacebookUser = true;
                await Shell.Current.Navigation.PopAsync();

                try
                {
                    await login(FBProfile.Email, "FB" + FBProfile.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    IsFBSignUpMode = true;
                    if (Mode == RegisterMode.Loggin)
                    {
                        switchFormMode();
                    }

                    Email = FBProfile.Email;
                    Password = "FB" + FBProfile.Id;
                    ConfirmPassword = "FB" + FBProfile.Id;

                    Massage = "השלם את שאר שדות החובה";
                }

                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                object imageSource = imageSourceConverter.ConvertFromInvariantString(FBProfile.Picture.Data.Url);
                ImageSource imageSource1 = imageSource as ImageSource;
            }
        }

        public async Task UpdateCitiesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpMethod httpMethod = HttpMethod.Get;
                    string filters = string.Format(@"limit=20&offset=0&fields=שם_ישוב&distinct=true&sort=שם_ישוב&include_total=false&q={{""שם_ישוב"": ""{1}{0}""}}",
                        City.Contains(" ") == false ? ":*" : "", City);
                    if (City.Contains(" ") == false)
                    {
                        filters += "&plain=false";
                    }

                    string uri = "https://data.gov.il/api/action/datastore_search?resource_id=d4901968-dad3-4845-a9b0-a57d027f11ab" + "&" + filters;
                    HttpRequestMessage request = new HttpRequestMessage() { Method = httpMethod };
                    request.RequestUri = new Uri(uri);
                    HttpResponseMessage httpResponse = await client.SendAsync(request);
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    CitiesResult citiesResult = JsonConvert.DeserializeObject<CitiesResult>(responseContent);
                    Cities.Clear();
                    foreach (CityItem item in citiesResult.Result.Records)
                    {
                        Cities.Add(item.שם_ישוב);
                    }
                    if (citiesResult.Result.Records.Count == 0)
                    {
                        Cities.Add("אין תוצאות");
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public class CitiesResult
        {
            public Myclass Result { get; set; }
        }
        public class Myclass
        {
            public List<CityItem> Records { get; set; }
        }

        public class CityItem
        {
            public string שם_ישוב { get; set; }
        }

        private bool checkInputValidity()
        {
            if (Mode.Equals(RegisterMode.SignUp))
            {
                if (string.IsNullOrWhiteSpace(ConfirmPassword) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(PhoneNumber) ||
                    string.IsNullOrWhiteSpace(City) ||
                    string.IsNullOrWhiteSpace(UserName))
                {
                    Massage = "יש למלא את כל שדות החובה";
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Massage = "יש למלא את כל השדות";
                return false;
            }

            if (StringValidationService.IsValid(Email, ValidationType.Email) == false)
            {
                Massage = "כתובת אימייל לא תקינה";
                return false;
            }

            if (StringValidationService.IsValid(Password, ValidationType.Password) == false)
            {
                Massage = "יש להזין סיסמה בעלת לפחות 8 תווים הכוללים אותיות ומיספרים";
                return false;
            }

            if (Mode.Equals(RegisterMode.SignUp))
            {
                if (Password != ConfirmPassword)
                {
                    Massage = "הסיסמאות אינן תואמות";
                    return false;
                }
            }

            if (Mode.Equals(RegisterMode.SignUp) && !string.IsNullOrWhiteSpace(PhoneNumber))
            {

                if (StringValidationService.IsValid(PhoneNumber, ValidationType.PhoneNumber) == false)
                {
                    Massage = "מספר טלפון לא תקין";
                    return false;
                }
            }
            return true;
        }

        private void clearForm()
        {
            Mode = RegisterMode.Loggin;
            SubmitButtonText = "התחבר";
            LabelText = "עדיין לא הצטרפת?";
            SwitchFormButtonText = "הצטרף עכשיו.";
            Massage = "";
            Email = "";
            Password = "";
            ConfirmPassword = "";
            UserName = "";
            PhoneNumber = "";
            City = "";
            Neighborhood = "";
            IsFBSignUpMode = false;
        }
    }
}