using Swap.Chat_Database;
using Swap.Models;
using Swap.Services;
using Swap.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Swap.ViewModels
{
    public class MainChatViewModel : BaseViewModel
    {
        private ObservableCollection<Contact> m_Items;
        public ObservableCollection<Contact> Items
        {
            get { return m_Items; }
            set { SetValue(ref m_Items, value); }
        }

        private object m_SelectionItem;
        public object SelectionItem
        {
            get { return m_SelectionItem; }
            set { SetValue(ref m_SelectionItem, value); }
        }

        private ICommand m_SelectionChangedCommand;
        public ICommand SelectionChangedCommand => m_SelectionChangedCommand ?? (m_SelectionChangedCommand = new Command(async () =>
        {
            if (SelectionItem == null)
            {
                return;
            }

            Contact selectedItem = SelectionItem as Contact;
            ChatPage chatPage = new ChatPage(selectedItem);
            await Shell.Current.Navigation.PushAsync(chatPage);

            SelectionItem = null;
        }));

        public MainChatViewModel()
        {
            Items = new ObservableCollection<Contact>();
        }
    }

    public class Contact
    {
        public ImageSource ImageSource { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string ChatId { get; set; }
        public string LastMessage { get; set; }
        public string LastMessageDateTime { get; set; }

        public Contact(string i_Username, int i_userId, string i_chatId)
        {
            this.ImageSource = ImageSource.FromFile("noimage.png");
            this.UserName = i_Username;
            this.UserId = i_userId;
            this.ChatId = i_chatId;
        }

        public async Task GetImageSourceAsync()
        {
            try
            {
                LoginUserResult userResult = await ServerFacade.Users.GetUserInfoAsync(UserId);
                if (userResult.Images != null && userResult.Images.Count != 0)
                {
                    byte[] byteArray = Convert.FromBase64String(userResult.Images.First().BytesOfImage);
                    ImageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
                }
                else
                {
                    ImageSource = ImageSource.FromFile("noimage.png");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאה", "משהו השתבש נסה שנית", "אישור");
                ImageSource = ImageSource.FromFile("noimage.png");
            }
        }

        public Contact(string i_Username, int i_userId, string i_chatId, InstantMessage message)
            : this(i_Username, i_userId, i_chatId)
        {
            this.LastMessage = $"{message.UserName}: {message.Body}";
            this.LastMessageDateTime = message.Time.ToString();
        }

        public async Task SetImageSource()
        {
            try
            {
                ItemFormServices.Image mainImage = null;
                LoginUserResult contact = await ServerFacade.Users.GetUserInfoAsync(this.UserId);
                if (contact.Images.Count > 0)
                {
                    mainImage = contact.Images.First();
                    byte[] byteArray = Convert.FromBase64String(mainImage.BytesOfImage);
                    this.ImageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאה", "משהו השתבש נסה שנית", "אישור");
            }
        }
    }
}