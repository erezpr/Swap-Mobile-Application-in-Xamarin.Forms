using Swap.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomeViewModel ViewModel
        {
            get { return BindingContext as HomeViewModel; }
            set { BindingContext = value; }
        }

        public HomePage()
        {
            ViewModel = new HomeViewModel();
            InitializeComponent();
            _ = ViewModel.GetItemsFromServer();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (string.IsNullOrWhiteSpace((Application.Current as App).Token) == false)
            {
                signInMessage.HeightRequest = 0;
                signInMessage.IsVisible = false;
            }
            else
            {
                signInMessage.HeightRequest = 150;
                signInMessage.IsVisible = true;
            }
        }
    }
}