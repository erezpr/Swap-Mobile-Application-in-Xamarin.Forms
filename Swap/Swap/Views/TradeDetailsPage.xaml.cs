using Swap.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TradeDetailsPage : ContentPage
    {
        public NotificationItem NotificationItem { get; set; }
        
        public TradeDetailsViewModel ViewModel
        {
            get { return BindingContext as TradeDetailsViewModel; }
            set { BindingContext = value; }
        }

        public TradeDetailsPage(NotificationItem i_NotificationItem)
        {
            NotificationItem = i_NotificationItem;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel == null)
            {
                ViewModel = new TradeDetailsViewModel(NotificationItem);
                await ViewModel.GetItemsFromServer();
            }
        }
    }
}