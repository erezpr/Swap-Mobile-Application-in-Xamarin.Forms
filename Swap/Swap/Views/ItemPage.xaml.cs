using Swap.Behaviors;
using Swap.Models;
using Swap.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Swap.Services.ItemFormServices;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage
    {
        public Item Item { get; set; }
        public ImageSource Source { get; set; }

        public ItemViewModel ViewModel
        {
            get { return BindingContext as ItemViewModel; }
            set { BindingContext = value; }
        }
        
        public ItemPage(Item i_Item, LoginUserResult i_User)
        {
            Item = i_Item;
            ViewModel = new ItemViewModel(Item, i_User);
            (Application.Current as App).ItemViewModel = ViewModel;

            InitializeComponent();

            itemName.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            author.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            platform.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            sellerName.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            _ = ViewModel.IncrementItemViewsNumber(Item);
        }
    }
}