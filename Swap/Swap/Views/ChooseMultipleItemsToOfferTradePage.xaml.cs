using Swap.Behaviors;
using Swap.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseMultipleItemsToOfferTradePage : ContentPage
    {
        public int ItemId { get; private set; }
        public int CustomerId { get; private set; }

        public ChooseMultipleItemsToOfferTradePage(int i_ItemId, int i_CustomerId)
        {
            ItemId = i_ItemId;
            CustomerId = i_CustomerId;
            InitializeComponent();
        }

        public ChooseMultipleItemsToOfferTradeViewModel ViewModel
        {
            get { return (BindingContext as ChooseMultipleItemsToOfferTradeViewModel); }
            set { BindingContext = value; }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Yield();

            if (ViewModel == null)
            {
                ViewModel = new ChooseMultipleItemsToOfferTradeViewModel(ItemId, CustomerId);
                await ViewModel.GetItemsFromServer();
            }
        }

        private void Label_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Label label = sender as Label;

            if (label.Text != null && label.Behaviors.Count == 0)
            {
                label.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            }
        }
    }
}