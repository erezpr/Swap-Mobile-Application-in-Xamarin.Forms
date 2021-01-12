using Swap.Chat_Database;
using Swap.Enums;
using Swap.Models;
using Swap.Services;
using Swap.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;

namespace Swap.ViewModels
{
    public class NotificationViewModel : BaseViewModel
    {
        public event Action ModeChanged;

        private NotificationMode m_Mode;
        public NotificationMode Mode
        {
            get { return m_Mode; }
            set
            {
                SetValue(ref m_Mode, value);
                ModeChanged?.Invoke();
            }
        }

        private bool m_IsRefreshing = false;
        public bool IsRefreshing
        {
            get { return m_IsRefreshing; }
            set { SetValue(ref m_IsRefreshing, value); }
        }

        private ObservableCollection<NotificationItem> m_ReceivedNotificationList;
        public ObservableCollection<NotificationItem> ReceivedNotificationList
        {
            get { return m_ReceivedNotificationList; }
            set { SetValue(ref m_ReceivedNotificationList, value); }
        }

        private ObservableCollection<NotificationItem> m_SentNotificationList;
        public ObservableCollection<NotificationItem> SentNotificationList
        {
            get { return m_SentNotificationList; }
            set { SetValue(ref m_SentNotificationList, value); }
        }

        public NotificationViewModel()
        {
            ReceivedNotificationList = new ObservableCollection<NotificationItem>();
            SentNotificationList = new ObservableCollection<NotificationItem>();
            IsBusy = false;
        }

        public async Task UpdateNotificationListAsync()
        {
            IsBusy = true;
            IsRefreshing = true;
            ReceivedNotificationList.Clear();
            SentNotificationList.Clear();
            try
            {
                List<Trade> trades = await ServerFacade.Trades.GetNotificationListAsync(app.UserId);

                foreach (Trade trade in trades)
                {
                    TradeStatus status;
                    if (trade.Status == null)
                    {
                        status = TradeStatus.WaitingForAction;
                    }
                    else if (trade.Status == 0)
                    {
                        status = TradeStatus.Rejected;
                    }
                    else
                    {
                        status = TradeStatus.Accepted;
                    }

                    Item item = await ServerFacade.Items.GetItemInfoAsync(trade.ItemId);
                    if (app.UserId == trade.OfferedById)
                    {
                        LoginUserResult user = await ServerFacade.Users.GetUserInfoAsync(trade.OfferedToId);
                        SentNotificationList.Add(new NotificationItem
                        {
                            Status = status,
                            ImageUrl = GetImageSource(item, 0),
                            Trade = trade,
                            User = user,
                            Item = item
                        });
                    }
                    else
                    {
                        LoginUserResult user = await ServerFacade.Users.GetUserInfoAsync(trade.OfferedById);
                        ReceivedNotificationList.Add(new NotificationItem
                        {
                            Status = status,
                            ImageUrl = GetImageSource(item, 0),
                            Trade = trade,
                            User = user,
                            Item = item
                        });
                    }
                }
                app.ReceivedNotificationList = ReceivedNotificationList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await Shell.Current.DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "אישור");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private ICommand m_ShowCustomerItems;
        public ICommand ShowCustomerItems => m_ShowCustomerItems ?? (m_ShowCustomerItems = new Command(async item =>
        {
            IsBusy = true;

            try
            {
                int customerId = (item as NotificationItem).Trade.OfferedById;
                MyItemsPage otherUserItemsPage = new MyItemsPage(MyItemsPage.View.otherUserItems);
                app.RefreshRequired = false;
                await Shell.Current.Navigation.PushAsync(otherUserItemsPage);
                await otherUserItemsPage.DisplayItemsOfUser(customerId, "All");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await Shell.Current.DisplayAlert("הודעת מערכת", "משהו השתבש, נסה שנית מאוחר יותר.", "אישור");
            }
            finally
            {
                IsBusy = false;
            }

        }));

        private ICommand m_ShowChatPage;
        public ICommand ShowChatPage => m_ShowChatPage ?? (m_ShowChatPage = new Command(async item =>
        {
            IsBusy = true;

            try
            {
                int contactId = (item as NotificationItem).Trade.OfferedById;
                LoginUserResult contact = await ServerFacade.Users.GetUserInfoAsync(contactId);
                Database database = (Application.Current as App).DataBase;
                UserToGroup contactUserToGroup = database.UserToGroupTable.Get(u => u.UserId == contactId);
                string chatId = contactUserToGroup?.Guid.ToString();

                ChatPage chatpage = new ChatPage(new Contact(contact.FirstName, contact.Id, chatId));
                await Shell.Current.Navigation.PushAsync(chatpage);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await Shell.Current.DisplayAlert("הודעת מערכת", "משהו השתבש, נסה שנית מאוחר יותר.", "אישור");
            }
            finally
            {
                IsBusy = false;
            }

        }));

        private ICommand m_ShowOfferedList;
        public ICommand ShowOfferedList => m_ShowOfferedList ?? (m_ShowOfferedList = new Command(async item =>
        {
            IsBusy = true;

            try
            {
                Trade trade = (item as NotificationItem).Trade;
                await Shell.Current.Navigation.PushAsync(new ChooseOneItemToAcceptTradePage(trade));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await Shell.Current.DisplayAlert("הודעת מערכת", "משהו השתבש, נסה שנית מאוחר יותר.", "אישור");
            }
            finally
            {
                IsBusy = false;
            }
        }));



        private ICommand m_deleteCommand;
        public ICommand DeleteCommand => m_deleteCommand ?? (m_deleteCommand = new Command(item =>
        {
            IsBusy = true;
            ReceivedNotificationList.Remove(item as NotificationItem);
            IsBusy = false;
        }));

        private ICommand m_SetReceivedMode;
        public ICommand SetReceivedMode => m_SetReceivedMode ?? (m_SetReceivedMode = new Command(() =>
        {
            IsBusy = true;
            Mode = NotificationMode.Received;
            IsBusy = false;
        }));

        private ICommand m_ShowTradeDetailsPage;
        public ICommand ShowTradeDetailsPage => m_ShowTradeDetailsPage ?? (m_ShowTradeDetailsPage = new Command(async item =>
        {
            IsBusy = true;
            await Shell.Current.Navigation.PushAsync(new TradeDetailsPage(item as NotificationItem));
            IsBusy = false;
        }));

        private ICommand m_SetSentMode;
        public ICommand SetSentMode => m_SetSentMode ?? (m_SetSentMode = new Command(() =>
        {
            IsBusy = true;
            Mode = NotificationMode.Sent;
            IsBusy = false;
        }));

        private ICommand m_RefreshCommand;
        public ICommand RefreshCommand => m_RefreshCommand ?? (m_RefreshCommand = new Command(async () =>
        {
            await UpdateNotificationListAsync();
        }));

    }
    public class NotificationItem
    {
        public TradeStatus Status { get; set; }
        public ImageSource ImageUrl { get; set; }
        public Trade Trade { get; set; }
        public LoginUserResult User { get; set; }
        public Item Item { get; set; }
    }
}