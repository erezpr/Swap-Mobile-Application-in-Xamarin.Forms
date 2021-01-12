using Swap.Enums;
using Swap.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPage : ContentPage
    {
        public NotificationViewModel ViewModel
        {
            get { return BindingContext as NotificationViewModel; }
            set { BindingContext = value; }
        }
   
        public NotificationPage()
        {
            ViewModel = new NotificationViewModel();
            ViewModel.ModeChanged += modeChanged;
            InitializeComponent();
        }

        private void modeChanged()
        {
            switch (ViewModel.Mode)
            {
                case NotificationMode.Received:
                    {
                        receivedButtonFrame.BackgroundColor = Color.Red;
                        sentButtonFrame.BackgroundColor = Color.FromHex("039BE6");
                        receivedNotifications.IsVisible = true;
                        sentNotifications.IsVisible = false;
                        receivedButton.FontAttributes = FontAttributes.Bold;
                        sentButton.FontAttributes = FontAttributes.None;
                    }
                    break;
                case NotificationMode.Sent:
                    {
                        receivedButtonFrame.BackgroundColor = Color.FromHex("039BE6");
                        sentButtonFrame.BackgroundColor = Color.Red;
                        receivedNotifications.IsVisible = false;
                        sentNotifications.IsVisible = true;
                        receivedButton.FontAttributes = FontAttributes.None;
                        sentButton.FontAttributes = FontAttributes.Bold;
                    }
                    break;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Yield();
            App app = Application.Current as App;

            if (string.IsNullOrWhiteSpace(app.Token) == false && app.IsUserHaveNewNotificationMessage == true)
            {
                app.IsUserHaveNewNotificationMessage = false;
                await ViewModel.UpdateNotificationListAsync();
            }
        }
    }
}