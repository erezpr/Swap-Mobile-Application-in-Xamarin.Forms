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
    public partial class SearchResultsPage : ContentPage
    {
        private enum ViewOptions
        {
            List = 0,
            Squares = 1
        };

        private App app = Application.Current as App;
        private List<ItemFormServices.Item> m_Items;
        private bool m_PageHasAppeared = false;
        private ViewOptions m_CurrentView = ViewOptions.List;

        private void changeView_Button_Clicked(object sender, EventArgs e)
        {
            if (m_CurrentView == ViewOptions.List)
            {
                listStackLayout.IsVisible = false;
                squaresStackLayout.IsVisible = true;
                m_CurrentView = ViewOptions.Squares;
                listViewButton.IsVisible = true;
                squaresViewButton.IsVisible = false;
            }
            else if (m_CurrentView == ViewOptions.Squares)
            {
                squaresStackLayout.IsVisible = false;
                listStackLayout.IsVisible = true;
                m_CurrentView = ViewOptions.List;
                squaresViewButton.IsVisible = true;
                listViewButton.IsVisible = false;
            }
        }

        protected async override void OnAppearing()
        {
            if (m_PageHasAppeared == true)
                return;
            m_PageHasAppeared = true;
            await Navigation.PushAsync(new WaitingPage());
            await showResults();
            await Navigation.PopAsync();
        }

        public SearchResultsPage(List<ItemFormServices.Item> i_items)
        {
            InitializeComponent();
            m_Items = i_items;
        }

        private async Task showResults()
        {
            Grid grid = null;
            for (int i = 0; i < m_Items.Count; i++)
            {
                Frame frame = await makeNewFrame(i, ViewOptions.List);
                LoginUserResult user = null;
                try
                {
                    user = await ServerFacade.Users.GetUserInfoAsync(m_Items[i].IdCustomer);
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e);
                }

                int index = i;
                frame.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(async () =>
                    {
                        ItemPage itemPage = new ItemPage(m_Items[index], user);
                        await Navigation.PushAsync(itemPage);
                    })
                });

                listStackLayout.Children.Add(frame);

                Frame frame2 = await makeNewFrame(i, ViewOptions.Squares);
                frame2.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(async () =>
                    {
                        ItemPage itemPage = new ItemPage(m_Items[index], user);
                        await Navigation.PushAsync(itemPage);
                    })
                });

                if (i % 2 == 0)
                {
                    grid = new Grid() { FlowDirection = FlowDirection.RightToLeft };
                    squaresStackLayout.Children.Add(grid);
                }
                grid.Children.Add(frame2, i % 2, 0);

            }
        }

        private async Task<Frame> makeNewFrame(int i, ViewOptions i_viewOption)
        {
            Frame result = new Frame()
            {
                BackgroundColor = Color.PaleGoldenrod,
            };
           
            Grid newGrid = await makeNewGrid(i, i_viewOption);
            result.Content = newGrid;
            return result;
        }

        private async Task<string> getItemCity(int i_itemFoundUserId)
        {
            string result;
            HttpClient client = new HttpClient();
            string Url = "http://Vmedu184.mtacloud.co.il/user/getUser?id=" + i_itemFoundUserId.ToString();

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get
            };

            request.RequestUri = new Uri(Url);
            string Token = "Bearer " + (Application.Current as App).Token;
            request.Headers.Add("Authorization", Token);
            HttpResponseMessage httpResponse = await client.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            LoginUserResult user = JsonConvert.DeserializeObject<LoginUserResult>(responseContent);
            result = user.City;
            return result;
        }

        private async Task<Grid> makeNewGrid(int i, ViewOptions i_viewOption)
        {
            Grid result = new Grid()
            {
                HeightRequest = 135
            };

            StackLayout ItemDetailsStackLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Spacing = 0,
            };

            int myUserId = (Application.Current as App).UserId;
            int itemFoundUserId = m_Items[i].IdCustomer;
            double distanceInMeters = await getDistanceBetweenUsers(myUserId, itemFoundUserId);
            string city = await getItemCity(itemFoundUserId);

            Label nameLabel = new Label()
            {
                Text = m_Items[i].Name,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.RoyalBlue,
                FontSize = 18
            };

            Label cityAndDistanceLabel = new Label()
            {
                Text = city + " , " + string.Format("{0:0.0}", (distanceInMeters / 1000)) + " ק'מ ממך",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.RoyalBlue,
                FontSize = 18
            };

            Label genreLabel = new Label()
            {
                Text = "ג'אנר: " + m_Items[i].Genre,
                HorizontalOptions = LayoutOptions.Center
            };

            
            Image image = new Image()
            {
                Aspect = Aspect.AspectFill,
                Margin = new Thickness(0, 0, 0, 0)
            };

            if (m_Items[i].ImagesOfItem.Count == 0)
            {
                image.Source = "noimage.jpg";
            }
            else
            {
                byte[] byteArray = Convert.FromBase64String(m_Items[i].ImagesOfItem[0].BytesOfImage);
                ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
                image.Source = imageSource;
            }

            ItemDetailsStackLayout.Children.Add(nameLabel);
            ItemDetailsStackLayout.Children.Add(cityAndDistanceLabel);

            if (i_viewOption == ViewOptions.List)
            {
                if (m_Items[i] is ItemFormServices.Book)
                {
                    Label typeOfItemLabel = new Label()
                    {
                        Text = "סוג: " + "ספר",
                        HorizontalOptions = LayoutOptions.Center
                    };
                    string author = m_Items[i].GetType().GetProperty("Author").GetValue(m_Items[i], null) as string;

                    Label authorLabel = new Label()
                    {
                        Text = "מחבר: " + author,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    ItemDetailsStackLayout.Children.Add(typeOfItemLabel);
                    if (author != null)
                    {
                        ItemDetailsStackLayout.Children.Add(authorLabel);
                    }
                }
                else if (m_Items[i] is ItemFormServices.VideoGame)
                {
                    Label typeOfItemLabel = new Label()
                    {
                        Text = "סוג: " + "משחק וידאו",
                        HorizontalOptions = LayoutOptions.Center
                    };

                    ItemFormServices.GamingPlatform? platform = m_Items[i].GetType().GetProperty("Platform").GetValue(m_Items[i], null) as ItemFormServices.GamingPlatform?;

                    string console = ItemFormServices.StringToPlatform.FirstOrDefault(x => x.Value == platform).Key;
                    Label consoleLabel = new Label()
                    {
                        Text = "פלטפורמה: " + console,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    ItemDetailsStackLayout.Children.Add(typeOfItemLabel);
                    ItemDetailsStackLayout.Children.Add(consoleLabel);
                }

                if (m_Items[i].Genre != null)
                {
                    ItemDetailsStackLayout.Children.Add(genreLabel);
                }

                result.Children.Add(ItemDetailsStackLayout, 0, 0);
                result.Children.Add(image, 1, 0);
                result.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(3, GridUnitType.Star)
                });

                result.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }
            else if (i_viewOption == ViewOptions.Squares)
            {
                cityAndDistanceLabel.Text = city + "\n" + string.Format("{0:0.0}", (distanceInMeters / 1000)) + " ק'מ ממך";
                cityAndDistanceLabel.FontSize = 14;
                cityAndDistanceLabel.FontAttributes = FontAttributes.None;
                cityAndDistanceLabel.TextColor = Color.Black;
                result.Children.Add(image, 0, 0);
                result.Children.Add(ItemDetailsStackLayout, 0, 1);
                result.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(4, GridUnitType.Star)
                });
             
                result.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
                
                result.HeightRequest = 300;
            }

            return result;
        }

        private async Task<double> getDistanceBetweenUsers(int i_myUserId, int i_itemFoundUserId)
        {
            string data = string.Format("id={0}&id2={1}", i_myUserId, i_itemFoundUserId);
            HttpClient client = new HttpClient();

            string UrlGetDistance = "http://Vmedu184.mtacloud.co.il/user/getdistance" + "?" + data;
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(UrlGetDistance)
            };

            string Token = "Bearer " + (Application.Current as App).Token;
            request.Headers.Add("Authorization", Token);
            HttpResponseMessage httpResponse = await client.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            double distance = JsonConvert.DeserializeObject<double>(responseContent);
            return distance;
        }
    }
}