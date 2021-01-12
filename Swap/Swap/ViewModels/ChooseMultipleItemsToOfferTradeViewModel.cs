using Swap.Models;
using Swap.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;
using static Swap.ViewModels.HomeViewModel;

namespace Swap.ViewModels
{
    public class ChooseMultipleItemsToOfferTradeViewModel : BaseViewModel
    {
        public int CustomerId { get; private set; }

        public int ItemId { get; private set; }

        private string m_SelectedItemCounterTitle;
        public string SelectedItemCounterTitle
        {
            get { return m_SelectedItemCounterTitle; }
            set { SetValue(ref m_SelectedItemCounterTitle, value); }
        }

        private ObservableCollection<ImagedItem> m_MyItems;
        public ObservableCollection<ImagedItem> MyItems
        {
            get { return m_MyItems; }
            set { SetValue(ref m_MyItems, value); }
        }

        private IList<object> m_SelectedItems;
        public IList<object> SelectedItems
        {
            get { return m_SelectedItems; }
            set { SetValue(ref m_SelectedItems, value); }
        }

        public ChooseMultipleItemsToOfferTradeViewModel(int i_ItemId, int i_CustomerId)
        {
            ItemId = i_ItemId;
            CustomerId = i_CustomerId;
            MyItems = new ObservableCollection<ImagedItem>();
            SelectedItemCounterTitle = "בחר פריט/ים";
            SelectedItems = new List<object>();
        }

        public async Task GetItemsFromServer()
        {
            List<Item> items;
            items = await GetItems(app.UserId);

            for (int i = 0; i < items.Count; i++)
            {
                ImageSource imageSource = GetImageSource(items[i], 0);
                MyItems.Add(new ImagedItem
                {
                    Item = items[i],
                    ImageSource = imageSource,
                    ItemName = items[i].Name,
                    UploadDate = items[i].UploadDate.ToShortDateString()
                });
            }
        }

        private ICommand m_CloseCommand;
        public ICommand CloseCommand => m_CloseCommand ?? (m_CloseCommand = new Command(async () =>
        {
            await Shell.Current.Navigation.PopAsync();
        }));

        private ICommand m_Submit;
        public ICommand Submit => m_Submit ?? (m_Submit = new Command(async () =>
        {
            if (SelectedItems.Count == 0)
            {
                await Shell.Current.DisplayAlert("התראה", "לא נבחרו פריטים", "אישור");
                return;
            }

            bool answer = await Shell.Current.DisplayAlert("התראה", "האם אתה בטוח שאתה רוצה להציע את מוצר/ים בעבור הפריט?", "אישור", "ביטול");

            if (answer == true)
            {
                string itemsTotrade = string.Empty;
                foreach (object item in SelectedItems)
                {
                    itemsTotrade += (item as ImagedItem).Item.Id.ToString() + ",";
                }

                itemsTotrade = itemsTotrade.Remove(itemsTotrade.Length - 1);
                Trade trade = new Trade
                {
                    OfferedToId = CustomerId,
                    ItemId = ItemId,
                    OfferedById = app.UserId,
                    ItemsToTrade = itemsTotrade
                };

                try
                {
                    await ServerFacade.Trades.OfferTradeAsync(trade);
                    await Shell.Current.DisplayAlert("הודעה", "הבקשה נשלחה בהצלחה", "אישור");
                    app.IsUserHaveNewNotificationMessage = true;
                    await Shell.Current.Navigation.PopAsync();
                }
                catch (System.Exception)
                {
                    await Shell.Current.DisplayAlert("הודעת מערכת", "משהו השתבש, נסה שנית מאוחר יותר.", "אישור");
                }
            }
        }));

        private ICommand m_SelectionChangedCommand;
        public ICommand SelectionChangedCommand => m_SelectionChangedCommand ?? (m_SelectionChangedCommand = new Command(() =>
        {
            if (SelectedItems.Count != 0)
            {
                SelectedItemCounterTitle = string.Format("פריטים שנבחרו ({0})", SelectedItems.Count);
            }
            else
            {
                SelectedItemCounterTitle = "בחר פריט/ים";
            }
        }));
    }
}