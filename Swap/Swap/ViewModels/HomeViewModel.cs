using Swap.Enums;
using Swap.Models;
using Swap.Services;
using Swap.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;

namespace Swap.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private bool m_IsUserLogin;
        public bool IsUserLogin
        {
            get { return m_IsUserLogin; }
            set { SetValue(ref m_IsUserLogin, value); }
        }

        private ObservableCollection<ImagedItem> m_PopularBooks;
        public ObservableCollection<ImagedItem> PopularBooks
        {
            get { return m_PopularBooks; }
            set { SetValue(ref m_PopularBooks, value); }
        }

        private ObservableCollection<ImagedItem> m_PopularGames;
        public ObservableCollection<ImagedItem> PopularGames
        {
            get { return m_PopularGames; }
            set { SetValue(ref m_PopularGames, value); }
        }

        public HomeViewModel()
        {
            PopularBooks = new ObservableCollection<ImagedItem>();
            PopularGames = new ObservableCollection<ImagedItem>();
            var a = new ImagedItem { ImageSource = ImageSource.FromResource("Swap.Images.spinner.gif") };
            PopularBooks.Add(a);
            PopularBooks.Add(a);
            PopularBooks.Add(a);
            PopularBooks.Add(a);
            PopularBooks.Add(a);
            PopularBooks.Add(a);

            PopularGames.Add(a);
            PopularGames.Add(a);
            PopularGames.Add(a);
            PopularGames.Add(a);
            PopularGames.Add(a);
            PopularGames.Add(a);
            IsBusy = false;
        }

        public async Task GetItemsFromServer()
        {
            try
            {
                IsBusy = true;
                ImagedItem item = new ImagedItem { ImageSource = ImageSource.FromResource("Swap.Images.spinner.gif") };
                int numberOfBooks = PopularBooks.Count;
                int numberOfGames = PopularGames.Count;
                PopularBooks.Clear();
                PopularGames.Clear();

                for (int i = 0; i < numberOfBooks; i++)
                { 
                    PopularBooks.Add(item);
                }

                for (int i = 0; i < numberOfGames; i++)
                {
                    PopularGames.Add(item);
                }

                List<Item> items = await ServerFacade.Items.GetMostViewedItems((int)ItemType.Book, 10);
                ObservableCollection<ImagedItem> popularBooksTemp = new ObservableCollection<ImagedItem>();
                for (int i = 0; i < items.Count; i++)
                {
                    ImageSource imageSource = GetImageSource(items[i], 0);
                    popularBooksTemp.Add(new ImagedItem
                    {
                        Item = items[i],
                        ImageSource = imageSource,
                        ItemName = items[i].Name,
                        Genre = items[i].Genre,
                        UploadDate = items[i].UploadDate.ToString("dd/MM/yy", CultureInfo.InvariantCulture),
                        ShowItemDetailsCommand = ShowItemDetailsCommand
                    });
                }

                PopularBooks.Clear();
                PopularBooks = popularBooksTemp;
                ObservableCollection<ImagedItem> popularsGamesTemp = new ObservableCollection<ImagedItem>();

                items = await ServerFacade.Items.GetMostViewedItems((int)ItemType.VideoGame, 10);
                for (int i = 0; i < items.Count; i++)
                {
                    ImageSource imageSource = GetImageSource(items[i], 0);
                    popularsGamesTemp.Add(new ImagedItem
                    {
                        Item = items[i],
                        ImageSource = imageSource,
                        ItemName = items[i].Name,
                        Genre = items[i].Genre,
                        UploadDate = items[i].UploadDate.ToString("dd/MM/yy", CultureInfo.InvariantCulture),
                        ShowItemDetailsCommand = ShowItemDetailsCommand
                    });
                }

                PopularGames.Clear();
                PopularGames = popularsGamesTemp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאת מערכת", "נסה מאוחר יותר", "אישור");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private ICommand m_ShowItemDetailsCommand;
        public ICommand ShowItemDetailsCommand => m_ShowItemDetailsCommand ?? (m_ShowItemDetailsCommand = new Command(async i_Item =>
        {
            try
            {
                IsBusy = true;
                LoginUserResult user = null;
                user = await ServerFacade.Users.GetUserInfoAsync((i_Item as Item).IdCustomer);
                await Shell.Current.Navigation.PushAsync(new ItemPage((Item)i_Item, user));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }

        }));

        private ICommand m_PushRegiterPage;
        public ICommand PushRegiterPage => m_PushRegiterPage ?? (m_PushRegiterPage = new Command(async () =>
        {
            await Shell.Current.GoToAsync("//register");
        }));

        private ICommand m_RefreshCommand;
        public ICommand RefreshCommand => m_RefreshCommand ?? (m_RefreshCommand = new Command(async () =>
        {
            await GetItemsFromServer();
        }));

        public class ImagedItem
        {
            public Item Item { get; set; }
            public ImageSource ImageSource { get; set; }
            public string ItemName { get; set; }
            public string Genre { get; set; }
            public string UploadDate { get; set; }
            public ICommand ShowItemDetailsCommand { get; set; }
        }
    }
}