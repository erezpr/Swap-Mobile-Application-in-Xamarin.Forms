using Swap.Chat_Database;
using Swap.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainChatPage : ContentPage
    {
        private App m_MyApp = (Application.Current as App);

        public MainChatViewModel ViewModel
        {
            get { return (BindingContext as MainChatViewModel); }
            set { BindingContext = value; }
        }
        public MainChatPage()
        {
            ViewModel = new MainChatViewModel();
            InitializeComponent();
        }

        bool m_UserHasLogedIn;

        protected override async void OnAppearing()
        {
            m_UserHasLogedIn = !string.IsNullOrWhiteSpace(m_MyApp.Token);
            if (!m_UserHasLogedIn)
            {
                await DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "OK");
                await Shell.Current.GoToAsync("//register");
            }

            await showContactsAsync();
        }

        private async Task showContactsAsync()
        {
            ViewModel.Items.Clear();
            Database database = (Application.Current as App).DataBase;
            Dictionary<int, UserToGroup> idToUserToGroup = database.UserToGroupTable.GetAll(u => u.UserId != m_MyApp.UserId)
                .Select(u => new KeyValuePair<int, UserToGroup>(u.UserId, u)).ToDictionary(x => x.Key, x => x.Value);

            if (idToUserToGroup.Any())
            {
                foreach (KeyValuePair<int, UserToGroup> keyValue in idToUserToGroup)
                {
                    if (database.UserToGroupTable.TryGetChatId(m_MyApp.UserId, keyValue.Key, out string chatId))
                    {
                        InstantMessage message = database.InstantMessageTable.GetLastMessageOfChat(chatId);
                        if (null == message)
                            continue;

                        UserToGroup current = keyValue.Value;
                        Contact contact = new Contact(current.Username, current.UserId, chatId, message);
                        await contact.GetImageSourceAsync();
                        ViewModel.Items.Add(contact);
                    }
                }
            }
        }
    }
}