using Swap.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;
using static Swap.ViewModels.HomeViewModel;

namespace Swap.ViewModels
{
    public class TradeDetailsViewModel : BaseViewModel
    {
        private ObservableCollection<ImagedItem> m_Items;
        public ObservableCollection<ImagedItem> Items
        {
            get { return m_Items; }
            set { SetValue(ref m_Items, value); }
        }
        public NotificationItem NotificationItem { get; set; }

        private ImagedItem m_RequestedItem;
        public ImagedItem RequestedItem
        {
            get { return m_RequestedItem; }
            set { SetValue(ref m_RequestedItem, value); }
        }
       
        public TradeDetailsViewModel(NotificationItem i_NotificationItem)
        {
            Items = new ObservableCollection<ImagedItem>();
            NotificationItem = i_NotificationItem;
        }

        public async Task GetItemsFromServer()
        {
            try
            {
                RequestedItem = new ImagedItem
                {
                    UploadDate = NotificationItem.Item.UploadDate.ToString("dd/MM/yy", CultureInfo.InvariantCulture),
                    ItemName = NotificationItem.Item.Name,
                    ImageSource = GetImageSource(NotificationItem.Item, 0)
                };
                if (NotificationItem.Trade.Status == null || NotificationItem.Trade.Status == 0)
                {
                    List<int> ItemsId = NotificationItem.Trade.ItemsToTrade.Split(',').Select(int.Parse).ToList();
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
                else
                {
                    Item item = await ServerFacade.Items.GetItemInfoAsync(NotificationItem.Trade.Status ?? 0);
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
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private ICommand m_PushItemDetailsPage;
        public ICommand PushRegiterTabPage => m_PushItemDetailsPage ?? (m_PushItemDetailsPage = new Command(() =>
        {
        }));
    }
}