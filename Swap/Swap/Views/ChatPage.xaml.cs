using Swap.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage() { }

        public ChatPage(Contact i_contact)
        {
            InitializeComponent();
            ViewModel = new ChatViewModel(i_contact);
        }

        public ChatViewModel ViewModel
        {
            get { return (BindingContext as ChatViewModel); }
            set { BindingContext = value; }
        }
    }
}