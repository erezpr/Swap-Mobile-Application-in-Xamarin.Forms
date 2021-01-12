using Swap.Behaviors;
using Swap.Models;
using Swap.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseOneItemToAcceptTradePage : ContentPage
    {
        public List<int> ItemsId { get; private set; }
        public Trade Trade { get; private set; }

        public ChooseOneItemToAcceptTradeViewModel ViewModel
        {
            get { return (BindingContext as ChooseOneItemToAcceptTradeViewModel); }
            set { BindingContext = value; }
        }

        public ChooseOneItemToAcceptTradePage(Trade i_Trade)
        {
            InitializeComponent();
            Trade = i_Trade;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel == null)
            {
                ViewModel = new ChooseOneItemToAcceptTradeViewModel(Trade);
                await ViewModel.GetItemsFromServer();
            }
        }

        private void Label_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Label label = sender as Label;

            if (label.Text != null && label.Behaviors.Count == 0)
            {
                label.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            }
        }
    }
}