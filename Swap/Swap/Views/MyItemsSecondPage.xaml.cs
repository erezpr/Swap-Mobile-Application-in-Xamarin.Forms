using Swap.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace Swap.Views
{
    internal enum ViewOptions
    {
        ThreeImagesPerRow = 0,
        FourImagesPerRow = 1
    };

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyItemsSecondPage : ContentPage
    {
        private MyItemsPage m_ItemsPage;
        private static List<ItemFormServices.Book> m_Bookitems = new List<ItemFormServices.Book>();
        private static List<ItemFormServices.VideoGame> m_VideoGameitems = new List<ItemFormServices.VideoGame>();
        private List<Grid> m_ThreeItemsArowGrids = new List<Grid>();
        private List<Grid> m_FourItemsArowGrids = new List<Grid>();

        internal readonly Dictionary<string, StackLayout> r_VideoGameGenreToStackLayout = new Dictionary<string, StackLayout>();
        internal readonly Dictionary<string, StackLayout> r_VideoGamePlatformToStackLayout = new Dictionary<string, StackLayout>();
        internal readonly Dictionary<string, StackLayout> r_BooksGenreToStackLayout = new Dictionary<string, StackLayout>();
        private ViewOptions m_ImagesView = ViewOptions.ThreeImagesPerRow;
        private bool m_PageAlreadyBeenSortedByVideoGameGenre = false;
        private bool m_PageAlreadyBeenSortedByBooksGenre = false;
        private bool m_PageAlreadyBeenSortedByPlatform = false;

        private string m_Category;
        private bool m_PageHasAppeared = false;
        private string m_CurrentPage;

        protected override void OnAppearing()
        {

            if (m_PageHasAppeared == true)
                return;
            m_PageHasAppeared = true;

            if (m_Category == "All")
            {
                if (m_CurrentPage == "Video Game")
                {
                    VideoGamesViewOptionsGrid.IsVisible = true;
                    InitializeGrids("VideoGame");
                }
                else if (m_CurrentPage == "Book")
                {
                    BooksViewOptionsGrid.IsVisible = true;
                    InitializeGrids("Book");
                }
            }
            else
            {
                if (m_CurrentPage == "Video Game")
                {
                    List<ItemFormServices.VideoGame> originalList = m_VideoGameitems.ToList();
                    removeAllItemsOutOfCategory("Video Game", m_Category);
                    InitializeGrids("VideoGame");
                    m_VideoGameitems = originalList;
                }
                else if (m_CurrentPage == "Book")
                {
                    List<ItemFormServices.Book> originalList2 = m_Bookitems.ToList();
                    removeAllItemsOutOfCategory("Book", m_Category);
                    InitializeGrids("Book");
                    m_Bookitems = originalList2;
                }
            }
        }

        //------------------constructors---------------------//

        internal MyItemsSecondPage(MyItemsPage i_myItemsPage, string i_categoryOption, List<ItemFormServices.Book> i_Bookitems)
        {
            InitializeComponent();
            m_Bookitems = i_Bookitems;
            m_ItemsPage = i_myItemsPage;
            m_Category = i_categoryOption;
            m_CurrentPage = "Book";
        }

        internal MyItemsSecondPage(MyItemsPage i_myItemsPage, string i_categoryOption, List<ItemFormServices.VideoGame> i_VideoGameitems)
        {
            InitializeComponent();
            m_VideoGameitems = i_VideoGameitems;
            m_ItemsPage = i_myItemsPage;
            m_Category = i_categoryOption;
            m_CurrentPage = "Video Game";
        }
        //-----------------------------------------------------//

        private void removeAllItemsOutOfCategory(string i_type, string i_CategoryOption)
        {
            switch (i_type)
            {
                case "Book":
                    foreach (ItemFormServices.Book bookItem in m_Bookitems.ToList())
                    {
                        if (bookItem.Genre != i_CategoryOption)
                        {
                            m_Bookitems.Remove(bookItem);
                        }
                    }
                    break;
                case "Video Game":

                    foreach (ItemFormServices.VideoGame videoGameitem in m_VideoGameitems.ToList())
                    {

                        ItemFormServices.GamingPlatform gamingPlatform = videoGameitem.Platform;
                        string platform = ItemFormServices.StringToPlatform.FirstOrDefault(x => x.Value == gamingPlatform).Key;
                        if ((videoGameitem.Genre != i_CategoryOption) && (platform != i_CategoryOption))
                        {
                            m_VideoGameitems.Remove(videoGameitem);
                        }
                    }
                    break;
            }
        }

        private void InitializeGrids(string i_Type)
        {

            int listOfItemsLength = 0;
            if (i_Type == "Book")
            {
                listOfItemsLength = m_Bookitems.Count;
            }
            else if (i_Type == "VideoGame")
            {
                listOfItemsLength = m_VideoGameitems.Count;
            }

            for (int i = 0; i < listOfItemsLength; i++)
            {
                m_ThreeItemsArowGrids.Add(new Grid());
                m_FourItemsArowGrids.Add(new Grid());

                m_ThreeItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                m_ThreeItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                m_ThreeItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                m_FourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                m_FourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                m_FourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                m_FourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                Grid bigImageContainer = null;
                Grid smallImageContainer = null;
                if (i_Type == "Book")
                {
                    smallImageContainer = m_ItemsPage.makeGridImageContainer(m_Bookitems[i], 100, 180, 20);
                    bigImageContainer = m_ItemsPage.makeGridImageContainer(m_Bookitems[i], 130, 250, 20);
                }
                else if (i_Type == "VideoGame")
                {
                    smallImageContainer = m_ItemsPage.makeGridImageContainer(m_VideoGameitems[i], 100, 180, 20);
                    bigImageContainer = m_ItemsPage.makeGridImageContainer(m_VideoGameitems[i], 130, 250, 20);
                }

                m_ThreeItemsArowGrids[i / 3].FlowDirection = FlowDirection.RightToLeft;
                m_ThreeItemsArowGrids[i / 3].Children.Add(bigImageContainer, i % 3, 0);
                m_FourItemsArowGrids[i / 4].FlowDirection = FlowDirection.RightToLeft;
                m_FourItemsArowGrids[i / 4].Children.Add(smallImageContainer, i % 4, 0);
            }

            for (int i = 0; i < m_ThreeItemsArowGrids.Count; i++)
            {
                threeInArowMainStackLayout.Children.Add(m_ThreeItemsArowGrids[i]);
            }

            for (int i = 0; i < m_FourItemsArowGrids.Count; i++)
            {
                fourInArowMainStackLayout.Children.Add(m_FourItemsArowGrids[i]);
            }
        }

        private void changeView_Button_Clicked(object sender, EventArgs e)
        {
            if (m_ImagesView == ViewOptions.ThreeImagesPerRow)
            {
                threeInArowMainStackLayout.IsVisible = false;
                fourInArowMainStackLayout.IsVisible = true;
                m_ImagesView = ViewOptions.FourImagesPerRow;
                threeUpButton.IsVisible = true;
                fourUpButton.IsVisible = false;
            }
            else if (m_ImagesView == ViewOptions.FourImagesPerRow)
            {
                fourInArowMainStackLayout.IsVisible = false;
                threeInArowMainStackLayout.IsVisible = true;
                m_ImagesView = ViewOptions.ThreeImagesPerRow;
                fourUpButton.IsVisible = true;
                threeUpButton.IsVisible = false;
            }
        }
        //---------------Video Games Event Handlers---------------//

        private void viewAllVideoGames_Button_Clicked(object sender, EventArgs e)
        {
            viewAllMainStackLayout.IsVisible = true;
            viewByVideoGameGenreMainStackLayout.IsVisible = false;
            viewByPlatformMainStackLayout.IsVisible = false;
            viewAllVideoGamesButton.FontAttributes = FontAttributes.Bold;
            viewAllVideoGamesButton.BorderWidth = 1;
            viewByVideoGameGenreButton.BorderWidth = viewByPlatformButton.BorderWidth = 0;
            viewByVideoGameGenreButton.FontAttributes = viewByPlatformButton.FontAttributes = FontAttributes.None;
        }

        private void viewVideoGamesByGenre_Button_clicked(object sender, EventArgs e)
        {
            viewByVideoGameGenreMainStackLayout.IsVisible = true;
            viewAllMainStackLayout.IsVisible = false;
            viewByPlatformMainStackLayout.IsVisible = false;
            viewByVideoGameGenreButton.FontAttributes = FontAttributes.Bold;
            viewByVideoGameGenreButton.BorderWidth = 1;
            viewAllVideoGamesButton.BorderWidth = viewByPlatformButton.BorderWidth = 0;
            viewAllVideoGamesButton.FontAttributes = viewByPlatformButton.FontAttributes = FontAttributes.None;

            if (m_PageAlreadyBeenSortedByVideoGameGenre == false)
            {
                viewPageSortedBy("VideoGame Genre");
                m_PageAlreadyBeenSortedByVideoGameGenre = true;
            }
        }

        private void viewByPlatform_Button_clicked(object sender, EventArgs e)
        {
            viewByPlatformMainStackLayout.IsVisible = true;
            viewByVideoGameGenreMainStackLayout.IsVisible = false;
            viewAllMainStackLayout.IsVisible = false;
            viewByPlatformButton.FontAttributes = FontAttributes.Bold;
            viewByPlatformButton.BorderWidth = 1;
            viewByVideoGameGenreButton.BorderWidth = viewAllVideoGamesButton.BorderWidth = 0;
            viewByVideoGameGenreButton.FontAttributes = viewAllVideoGamesButton.FontAttributes = FontAttributes.None;

            if (m_PageAlreadyBeenSortedByPlatform == false)
            {
                viewPageSortedBy("Platform");
                m_PageAlreadyBeenSortedByPlatform = true;
            }
        }
        //---------------Books View Event Handlers---------------//

        private void viewAllBooks_Button_Clicked(object sender, EventArgs e)
        {
            viewAllMainStackLayout.IsVisible = true;
            viewByBooksGenreMainStackLayout.IsVisible = false;
            viewAllBooksButton.FontAttributes = FontAttributes.Bold;
            viewAllBooksButton.BorderWidth = 1;
            viewByBooksGenreButton.BorderWidth = 0;
            viewByBooksGenreButton.FontAttributes = FontAttributes.None;
        }

        private void viewBooksByGenre_Button_clicked(object sender, EventArgs e)
        {
            viewByBooksGenreMainStackLayout.IsVisible = true;
            viewAllMainStackLayout.IsVisible = false;
            viewByBooksGenreButton.FontAttributes = FontAttributes.Bold;
            viewByBooksGenreButton.BorderWidth = 1;
            viewAllBooksButton.BorderWidth = 0;
            viewAllBooksButton.FontAttributes = FontAttributes.None;

            if (m_PageAlreadyBeenSortedByBooksGenre == false)
            {
                viewPageSortedBy("Book Genre");
                m_PageAlreadyBeenSortedByBooksGenre = true;
            }
        }
        //-----------------------------------------------------//
        private void viewPageSortedBy(string i_sortingCategory)
        {
            if (i_sortingCategory == "VideoGame Genre")
            {
                for (int i = 0; i < m_VideoGameitems.Count; i++)
                {
                    string genre = m_VideoGameitems[i].Genre;
                    sortBy("Video Game", i, genre, r_VideoGameGenreToStackLayout, viewByVideoGameGenreMainStackLayout);
                }
            }
            else if (i_sortingCategory == "Platform")
            {
                for (int i = 0; i < m_VideoGameitems.Count; i++)
                {
                    ItemFormServices.GamingPlatform gamingPlatform = m_VideoGameitems[i].Platform;
                    string platform = ItemFormServices.StringToPlatform.FirstOrDefault(x => x.Value == gamingPlatform).Key;
                    sortBy("Video Game", i, platform, r_VideoGamePlatformToStackLayout, viewByPlatformMainStackLayout);
                }
            }
            else if (i_sortingCategory == "Book Genre")
            {
                for (int i = 0; i < m_Bookitems.Count; i++)
                {
                    string genre = m_Bookitems[i].Genre;
                    sortBy("Book", i, genre, r_BooksGenreToStackLayout, viewByBooksGenreMainStackLayout);
                }
            }
        }

        private void sortBy(string i_type, int i, string i_Category, Dictionary<string, StackLayout> i_Dictionary, StackLayout i_MainStackLayout)
        {
            if (i_Dictionary.ContainsKey(i_Category) == false)
            {
                StackLayout stackLayout = new StackLayout()
                {
                    HeightRequest = 200,
                    Orientation = StackOrientation.Horizontal
                };

                Frame frame = new Frame()
                {
                    BorderColor = Color.WhiteSmoke,
                    Content = stackLayout

                };

                ScrollView result = new ScrollView()
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    Orientation = ScrollOrientation.Horizontal,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                    Content = frame
                };

                i_Dictionary.Add(i_Category, stackLayout);

                Button button = new Button()
                {
                    Text = "הצג הכל",
                    FontSize = 12,
                    TextColor = Color.DarkOrange,
                    BackgroundColor = Color.White,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start
                };

                button.Clicked += async (sender, e) =>
                {
                    if (i_type == "Book")
                    {
                        await Navigation.PushAsync(new MyItemsSecondPage(m_ItemsPage, i_Category, m_Bookitems));
                    }
                    else if (i_type == "Video Game")
                    {
                        await Navigation.PushAsync(new MyItemsSecondPage(m_ItemsPage, i_Category, m_VideoGameitems));
                    }
                };

                Label label = ItemFormServices.SetNewLabel(i_Category);
                label.HorizontalOptions = LayoutOptions.EndAndExpand;
                label.VerticalOptions = LayoutOptions.EndAndExpand;

                StackLayout titleStackLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                titleStackLayout.Children.Add(button);
                titleStackLayout.Children.Add(label);
                i_MainStackLayout.Children.Add(titleStackLayout);
                i_MainStackLayout.Children.Add(result);
            }

            Grid imageContainer = null;
            if (i_type == "Book")
            {
                imageContainer = m_ItemsPage.makeGridImageContainer(m_Bookitems[i], 100, 180, 20);
            }
            else if (i_type == "Video Game")
            {
                imageContainer = m_ItemsPage.makeGridImageContainer(m_VideoGameitems[i], 100, 180, 20);
            }

            i_Dictionary[i_Category].Children.Add(imageContainer);
        }
    }
}