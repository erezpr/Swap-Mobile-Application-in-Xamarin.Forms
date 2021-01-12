using Swap.ContentViews;
using Swap.Services;
using Swap.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterViewModel ViewModel
        {
            get { return (BindingContext as RegisterViewModel); }
            set { BindingContext = value; }
        }

        public RegisterPage()
        {
            InitializeComponent();
            ViewModel = new RegisterViewModel(this);
            ViewModel.ModeChanged += changeModeAnimationAsync;
            logo.Source = ImageSource.FromResource("Swap.Images.logo.png");
        }

        public async Task<bool> AskUserToTurnOnGPS()
        {
            bool userDesicision = await DisplayAlert("שגיאה", "אנא הדלק את כפתור הGPS\n לאחר תהליך הרישום תוכל לכבות אותו בחזרה.", "OK", "CANCEL");
            if (userDesicision)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    DependencyService.Get<ILocationService>().OpenSettings();
                }
            }
            
            return userDesicision;
        }

        private void changeModeAnimationAsync()
        {
            switch (ViewModel.Mode)
            {
                case Enums.RegisterMode.Loggin:
                    {
                        var animate = new Animation(d => signInFrame.HeightRequest = d, 410, 0,  Easing.SinOut, new Action(()=> 
                        {
                            signInFrame.IsVisible = false;
                        }));
                        animate.Commit(signInFrame, "ButtonGraph1", 32, 600);
                    }
                    break;
                case Enums.RegisterMode.SignUp:
                    {
                        signInFrame.IsVisible = true;
                        var animate = new Animation(d => signInFrame.HeightRequest = d, 0, 410, Easing.SinOut);
                        animate.Commit(signInFrame, "ButtonGraph1", 32, 600);
                    }
                    break;
            }
        }

        private async void ImageEntryContentView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                if (string.IsNullOrWhiteSpace((sender as ImageEntryContentView).Text) == false)
                {
                    ViewModel.Cities.Clear();

                    if (CityPiker.HeightRequest == 0)
                    {
                        CityPiker.IsVisible = true;
                        var animate = new Animation(d => CityPiker.HeightRequest = d, 0, 150, Easing.SinOut);
                        animate.Commit(CityPiker, "ButtonGraph2", 32, 600);
                        animate = new Animation(d => signInFrame.HeightRequest = d, 410, 560, Easing.SinOut);
                        animate.Commit(signInFrame, "ButtonGraph2", 32, 600);
                    }
                    await ViewModel.UpdateCitiesAsync();
                }
                else
                {
                    if (CityPiker.HeightRequest != 0)
                    {
                        var animate = new Animation(d => CityPiker.HeightRequest = d, 150, 0, Easing.SinOut);
                        animate.Commit(CityPiker, "ButtonGraph3", 32, 600);
                        animate = new Animation(d => signInFrame.HeightRequest = d, 560, 410, Easing.SinOut);
                        animate.Commit(signInFrame, "ButtonGraph3", 32, 600, Easing.SinInOut, (s, d) =>
                        {
                            ViewModel.Cities.Clear();
                            CityPiker.IsVisible = false;
                        });
                    }
                }
            }
        }

        private void CityPiker_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ViewModel.CitySelected.Execute(e.SelectedItem);
            var animate = new Animation(d => CityPiker.HeightRequest = d, 150, 0, Easing.SinOut);
            animate.Commit(CityPiker, "ButtonGraph4", 32, 600);
            animate = new Animation(d => signInFrame.HeightRequest = d, signInFrame.HeightRequest, signInFrame.HeightRequest - 150, Easing.SinOut);
            animate.Commit(signInFrame, "ButtonGraph4", 32, 600, Easing.SinOut, (s, d) =>
            {
                ViewModel.Cities.Clear();
                CityPiker.IsVisible = false;
            });
        }
    }
}