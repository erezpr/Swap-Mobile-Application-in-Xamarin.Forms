using Swap.Models;
using Swap.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;
using static Swap.ViewModels.HomeViewModel;

namespace Swap.ViewModels
{
    public class ChooseOneItemToAcceptTradeViewModel : BaseViewModel
    {
        private readonly Trade m_Trade;

        private string m_SelectedItemCounterTitle;
        public string SelectedItemCounterTitle
        {
            get { return m_SelectedItemCounterTitle; }
            set { SetValue(ref m_SelectedItemCounterTitle, value); }
        }

        private ObservableCollection<ImagedItem> m_Items;
        public ObservableCollection<ImagedItem> Items
        {
            get { return m_Items; }
            set { SetValue(ref m_Items, value); }
        }

        private object m_SelectedItem;
        public object SelectedItem
        {
            get { return m_SelectedItem; }
            set { SetValue(ref m_SelectedItem, value); }
        }

        public ChooseOneItemToAcceptTradeViewModel(Trade i_Trade)
        {
            Items = new ObservableCollection<ImagedItem>();
            m_Trade = i_Trade;
        }

        public async Task GetItemsFromServer()
        {
            List<int> ItemsId = m_Trade.ItemsToTrade.Split(',').Select(int.Parse).ToList();
            foreach (int itemId in ItemsId)
            {
                Item item = await ServerFacade.Items.GetItemInfoAsync(itemId);
                ImageSource imageSource = GetImageSource(item, 0);

                Items.Add(new ImagedItem
                {
                    Item = item,
                    ImageSource = imageSource,
                    ItemName = item.Name,
                    UploadDate = item.UploadDate.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
                });
            }
        }

        private ICommand m_Submit;
        public ICommand Submit => m_Submit ?? (m_Submit = new Command(async () =>
        {
            if (SelectedItem == null)
            {
                await Shell.Current.DisplayAlert("התראה", "לא נבחרו פריטים", "אישור");
                return;
            }

            bool answer = await Shell.Current.DisplayAlert("התראה", "האם אתה בטוח שאתה רוצה להציע את מוצר/ים בעבור הפריט?", "אישור", "ביטול");

            if (answer == true)
            {
                string itemsTotrade = (SelectedItem as ImagedItem).Item.Id.ToString();

                Trade trade = new Trade
                {
                    Id = m_Trade.Id,
                    ItemId = m_Trade.ItemId,
                    OfferedById = m_Trade.OfferedById,
                    OfferedToId = m_Trade.OfferedToId,
                    ItemsToTrade = itemsTotrade
                };

                try
                {
                    await ServerFacade.Trades.AnswerTradeAsync(trade);
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

        private ICommand m_Refusal;
        public ICommand Refusal => m_Refusal ?? (m_Refusal = new Command(async () =>
        {
            bool answer = await Shell.Current.DisplayAlert("התראה", "האם אתה בטוח מעוניין לסרב להצעה", "אישור", "ביטול");

            if (answer == true)
            {
                Trade trade = new Trade
                {
                    Id = m_Trade.Id,
                    ItemId = 0,
                    OfferedById = m_Trade.OfferedById,
                    OfferedToId = m_Trade.OfferedToId
                };

                try
                {
                    await ServerFacade.Trades.AnswerTradeAsync(trade);
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
    }
}