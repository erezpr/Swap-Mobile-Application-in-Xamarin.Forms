using Swap.Enums;
using Swap.Models;
using Swap.Services;
using Swap.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;

namespace Swap.ViewModels
{
    public class ItemViewModel : BaseViewModel
    {
        public int CustomerId { get; private set; }
   
        public int ItemId { get; private set; }

        private ObservableCollection<Images> m_ImageList;
        public ObservableCollection<Images> ImageList
        {
            get { return m_ImageList; }
            set { SetValue(ref m_ImageList, value); }
        }

        private string m_ItemName;
        public string ItemName
        {
            get { return m_ItemName; }
            set { SetValue(ref m_ItemName, value); }
        }

        private string m_ItemGenre;
        public string ItemGenre
        {
            get { return m_ItemGenre; }
            set { SetValue(ref m_ItemGenre, value); }
        }

        private ItemType m_ItemType;
        public ItemType ItemType
        {
            get { return m_ItemType; }
            set { SetValue(ref m_ItemType, value); }
        }

        private string m_UploadDate;
        public string UploadDate
        {
            get { return m_UploadDate; }
            set { SetValue(ref m_UploadDate, value); }
        }

        private string m_Condition;
        public string Condition
        {
            get { return m_Condition; }
            set { SetValue(ref m_Condition, value); }
        }

        private string m_Author;
        public string Author
        {
            get { return m_Author; }
            set { SetValue(ref m_Author, value); }
        }

        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set { SetValue(ref m_Description, value); }
        }

        private int m_PagesNumber;
        public int PagesNumber
        {
            get { return m_PagesNumber; }
            set { SetValue(ref m_PagesNumber, value); }
        }

        private string m_Platform;
        public string Platform
        {
            get { return m_Platform; }
            set { SetValue(ref m_Platform, value); }
        }
        private string m_UserName;
        public string UserName
        {
            get { return m_UserName; }
            set { SetValue(ref m_UserName, value); }
        }
        private string m_UserPhoneNumber;
        public string UserPhoneNumber
        {
            get { return m_UserPhoneNumber; }
            set { SetValue(ref m_UserPhoneNumber, value); }
        }
        private string m_UserCity;
        public string UserCity
        {
            get { return m_UserCity; }
            set { SetValue(ref m_UserCity, value); }
        }


        public ItemViewModel(Item i_Item, LoginUserResult i_User)
        {
            ItemType = i_Item is Book ? ItemType.Book : ItemType.VideoGame;
            ImageList = new ObservableCollection<Images>();

            for (int i = 0; i < i_Item.ImagesOfItem?.Count; i++)
            {
                ImageSource imageSource = GetImageSource(i_Item, i);
                ImageList.Add(new Images { ImageSource = imageSource });
            }

            CustomerId = i_User.Id;
            ItemId = i_Item.Id;
            UserName = i_User.FirstName;
            UserPhoneNumber = i_User.CellPhone;
            UserCity = i_User.City;
            ItemName = i_Item.Name;
            ItemGenre = i_Item.Genre;
            Description = i_Item.Description;
            UploadDate = i_Item.UploadDate.ToString("dd/MM/yy", CultureInfo.InvariantCulture);
            Condition = ItemConditionToString[i_Item.Condition];

            if (i_Item is Book book)
            {
                Author = book.Author;
                PagesNumber = book.Pages;
            }

            if (i_Item is VideoGame videoGame)
            {
                Platform = PlatformToString[videoGame.Platform];
            }

            IsBusy = false;
        }

        public async Task IncrementItemViewsNumber(Item i_Item)
        {
            try
            {
                await ServerFacade.Items.IncrementItemViewsNumber(i_Item.Id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private ICommand m_OpenChooseMultipleItemsToOfferTradePageCommand;
        public ICommand OpenChooseMultipleItemsToOfferTradePageCommand => m_OpenChooseMultipleItemsToOfferTradePageCommand ?? (m_OpenChooseMultipleItemsToOfferTradePageCommand = new Command(async () =>
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(app.Token))
            {
                await Shell.Current.DisplayAlert("אינך מחובר", "לצורך הצעת החלפה את/ה חייב/ת להיות מחובר/ת", "אישור");
                IsBusy = false;
                return;
            }
            await Shell.Current.Navigation.PushAsync(new ChooseMultipleItemsToOfferTradePage(ItemId, CustomerId));

            IsBusy = false;
        }));

        private ICommand m_OpenMyItemsPageCommand;
        public ICommand OpenMyItemsPageCommand => m_OpenMyItemsPageCommand ?? (m_OpenMyItemsPageCommand = new Command(async () =>
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(app.Token))
            {
                await Shell.Current.DisplayAlert("אינך מחובר", "לצורך צפייה בפרטי הלקוח את/ה חייב/ת להיות מחובר/ת", "אישור");
                IsBusy = false;
                return;
            }
            MyItemsPage otherUserItemsPage = new MyItemsPage(MyItemsPage.View.otherUserItems);
            app.RefreshRequired = false;
            await Shell.Current.Navigation.PushAsync(otherUserItemsPage);
            await otherUserItemsPage.DisplayItemsOfUser(CustomerId, "All");

            IsBusy = false;
        }));
    }

    public class Images
    {
        public ImageSource ImageSource { get; set; }
    }
}