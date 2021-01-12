using FFImageLoading.Forms;
using Newtonsoft.Json;
using Swap.Models;
using Swap.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyItemsPage : ContentPage
    {
        private App app = Application.Current as App;
        private List<ItemFormServices.Item> m_Items;
        private List<ItemFormServices.Book> m_Bookitems = new List<ItemFormServices.Book>();
        private List<ItemFormServices.VideoGame> m_VideoGameitems = new List<ItemFormServices.VideoGame>();
        private ItemFormServices.Item m_LastItem;

        public enum View
        {
            myItems,
            otherUserItems
        }

        private View m_currentPageView;
        public MyItemsPage(View i_viewOption)
        {
            InitializeComponent();
            m_currentPageView = i_viewOption;
        }

        public MyItemsPage()
        {
            InitializeComponent();
            m_currentPageView = View.myItems;
        }

        protected override async void OnAppearing()
        {
            if (app.UserId == 0)
            {
                ClearAllSL();
                app.RefreshRequired = true;
            }
            else if (app.RefreshRequired == true)
            {
                await DisplayItemsOfUser(app.UserId, "All");
            }
        }

        internal Grid makeGridImageContainer(ItemFormServices.Item i_Item, int i_GridWidth, int i_ImageHeight, int i_TitleHeight)
        {
            Grid result = null;
            ItemFormServices.Image mainImage = null;
            ImageSource imageSource = null;
            if (i_Item.ImagesOfItem.Count > 0)
            {
                mainImage = i_Item.ImagesOfItem[0];
                var byteArray = Convert.FromBase64String(mainImage.BytesOfImage);
                imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
            }
            else  //if item has no images
            {
                imageSource = "noimage.jpg";
            }

            Grid imageContainer = new Grid()
            {
                WidthRequest = i_GridWidth
            };

            Label imageNumber_i_Title = new Label()
            {
                Text = i_Item.Name,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            CachedImage image = new CachedImage()
            {
                Aspect = Aspect.AspectFill,
                Source = imageSource,
            };

            image.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (m_currentPageView == View.myItems)
                    {
                        await Navigation.PushAsync(new ItemFormPage("Edit Item", this, i_Item));
                    }
                    else if (m_currentPageView == View.otherUserItems)
                    {
                        LoginUserResult user = null;
                        try
                        {
                            user = await ServerFacade.Users.GetUserInfoAsync(i_Item.IdCustomer);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }

                        await Navigation.PushAsync(new ItemPage(i_Item, user));
                    }
                })
            });

            imageContainer.Children.Add(image, 0, 0);
            imageContainer.Children.Add(imageNumber_i_Title, 0, 1);
            imageContainer.Opacity = 0;
            imageContainer.FadeTo(1, 2000);

            imageContainer.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(i_ImageHeight, GridUnitType.Absolute)
            });

            imageContainer.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(i_TitleHeight, GridUnitType.Absolute)
            });

            result = imageContainer;

            return result;
        }

        internal readonly Dictionary<ItemFormServices.Item, Grid> m_itemToGrid = new Dictionary<ItemFormServices.Item, Grid>();

        internal void addItemToStackLayout(ItemFormServices.Item i_Item)
        {
            Grid imageContainer = makeGridImageContainer(i_Item, 100, 180, 20);
            m_itemToGrid.Add(i_Item, imageContainer);

            if (i_Item is ItemFormServices.Book)
            {
                myBooksSL.Children.Add(imageContainer);
                m_Bookitems.Add(i_Item as ItemFormServices.Book);
            }
            else if (i_Item is ItemFormServices.VideoGame)
            {
                myVideoGamesSL.Children.Add(imageContainer);
                m_VideoGameitems.Add(i_Item as ItemFormServices.VideoGame);
            }
        }

        internal void removeItemFromStackLayout(ItemFormServices.Item i_ItemToEdit)
        {
            Grid imageContainer = m_itemToGrid[i_ItemToEdit];

            if (i_ItemToEdit is ItemFormServices.Book)
            {
                myBooksSL.Children.Remove(imageContainer);
                m_Bookitems.Remove(i_ItemToEdit as ItemFormServices.Book);
            }
            else if (i_ItemToEdit is ItemFormServices.VideoGame)
            {
                myVideoGamesSL.Children.Remove(imageContainer);
                m_VideoGameitems.Remove(i_ItemToEdit as ItemFormServices.VideoGame);
            }

            m_itemToGrid.Remove(i_ItemToEdit);
        }

        internal async Task DisplayItemsOfUser(int i_UserId, string i_Option)
        {
            if (i_UserId != app.UserId)
            {
                addItemButton.IsVisible = false;
            }

            await Navigation.PushAsync(new WaitingPage());

            app.RefreshRequired = false;
            var client = new HttpClient();
            var data = string.Format("id={0}", i_UserId);
            var httpMethod = HttpMethod.Get;
            string Token = "Bearer " + (Application.Current as App).Token;

            string UrlAllItems = "http://Vmedu184.mtacloud.co.il/item/getItems" + "?" + data;
            string UrlLastItem = "http://Vmedu184.mtacloud.co.il/item/getlastItem" + "?" + data;

            var request = new HttpRequestMessage()
            {
                Method = httpMethod,
            };

            if (i_Option == "All")
            {
                ClearAllSL();
                request.RequestUri = new Uri(UrlAllItems);
            }
            else if (i_Option == "Last Uploaded")
            {
                request.RequestUri = new Uri(UrlLastItem);
            }

            request.Headers.Add("Authorization", Token);
            HttpResponseMessage httpResponse = await client.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (i_Option == "All")
            {
                m_Items = JsonConvert.DeserializeObject<List<ItemFormServices.Item>>(responseContent);
                for (int i = 0; i < m_Items.Count; i++)
                {
                    addItemToStackLayout(m_Items[i]);
                }
            }

            else if (i_Option == "Last Uploaded")
            {
                m_LastItem = JsonConvert.DeserializeObject<ItemFormServices.Item>(responseContent);
                addItemToStackLayout(m_LastItem);
            }

            await Navigation.PopAsync();
        }

        private async void addItem_Button_Clicked(object sender, EventArgs e)
        {
            if ((Application.Current as App).UserId == 0)
            {
                await DisplayAlert("גישה לא מורשת", "עלייך להתחבר על מנת להוסיף פריטים לספריה שלך!", "אישור");
                return;
            }

            await Navigation.PushAsync(new ItemFormPage("Add Item", this));
        }

        private async void viewAllBookItems_Button_Clicked(object sender, EventArgs e)
        {
            if ((Application.Current as App).UserId == 0)
            {
                await DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "אישור");
                return;
            }

            List<ItemFormServices.Book> booksListcopy = m_Bookitems.ToList();
            await Navigation.PushAsync(new MyItemsSecondPage(this, "All", booksListcopy));
        }

        private async void viewAllVideoGameItems_Button_Clicked(object sender, EventArgs e)
        {
            if ((Application.Current as App).UserId == 0)
            {
                await DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "OK");
                return;
            }

            List<ItemFormServices.VideoGame> videoGamesListcopy = m_VideoGameitems.ToList();
            await Navigation.PushAsync(new MyItemsSecondPage(this, "All", videoGamesListcopy));
        }
        public void ClearAllSL()
        {
            myBooksSL.Children.Clear();
            myVideoGamesSL.Children.Clear();
        }
    }
}