using Plugin.Media;
using Plugin.Media.Abstractions;
using Swap.CustomViews;
using Swap.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Swap.Services.ItemFormServices;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemFormPage : ContentPage
    {
        public StackLayout BooksFormStackLayout { get; set; }
        public StackLayout VideoGamesFormStackLayout { get; set; }

        private bool m_BookFormHadBuilt = false;
        private bool m_VideoGameFormHadBuilt = false;

        private CustomPicker m_BookGenrePicker;
        private CustomEntry m_NumberOfPagesEntry, m_AuthorEntry;

        private CustomPicker m_PlatformPicker, m_VideoGameGenrePicker;

        private CustomEntry m_Description;
        private List<string> i_ImagesOfItem;
        internal readonly Dictionary<Xamarin.Forms.Image, string> m_ImageToString = new Dictionary<Xamarin.Forms.Image, string>();
        private Item m_Item;

        private Button m_OkButton;
        private StackLayout m_DoneEditingStackLayout;
        private StackLayout m_DeleteItemStackLayout;
        private MyItemsPage m_MyItemsPage;

        private bool m_OkButtonClickEnabled = true;
        private bool m_DoneEditingButtonClickEnabled = true;
        private bool m_DeleteItemButtonClickEnabled = true;

        //-----main methods-----//
        private void initializeCommonPickers()
        {
            foreach (string choice in state)
            {
                statePicker.Items.Add(choice);
            }

            foreach (string choice in typeOfItem)
            {
                typePicker.Items.Add(choice);
            }
        }

        private CustomEntry getDescriptionCustomEntry()
        {
            CustomEntry descriptionCustomEntry = new CustomEntry()
            {
                Placeholder = "פירוט נוסף",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 45
            };

            return descriptionCustomEntry;
        }

        private bool allRequiredFieldsHadBeenFilled()
        {
            if ((string.IsNullOrWhiteSpace(ItemName.Text) == true) || (statePicker.SelectedItem == null))
            {
                return false;
            }

            if ((string)typePicker.SelectedItem == "ספר")
            {
                if (m_BookGenrePicker.SelectedItem == null)
                {
                    return false;
                }
            }

            if ((string)typePicker.SelectedItem == "משחק וידאו")
            {
                if (m_PlatformPicker.SelectedItem == null || m_VideoGameGenrePicker.SelectedItem == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void updateItem()
        {
            if (m_Item is Book)
            {
                (m_Item as Book).Author = m_AuthorEntry.Text;
                if (m_NumberOfPagesEntry.Text != null)
                {
                    (m_Item as Book).Pages = short.Parse(m_NumberOfPagesEntry.Text);
                }

                m_Item.Genre = (string)m_BookGenrePicker.SelectedItem;
            }
            else
            {
                m_Item = new VideoGame();
                (m_Item as VideoGame).Genre = (string)m_VideoGameGenrePicker.SelectedItem;
                (m_Item as VideoGame).Platform = StringToPlatform[(string)m_PlatformPicker.SelectedItem];
            }
        }

        private Item getItem()
        {
            Item item;
            string selectedTypeOfItem = (string)typePicker.SelectedItem;

            if (selectedTypeOfItem == "ספר" || m_Item is Book)
            {
                item = new Book();
                (item as Book).Author = m_AuthorEntry.Text;
                if (m_NumberOfPagesEntry.Text != null)
                {
                    (item as Book).Pages = short.Parse(m_NumberOfPagesEntry.Text);
                }

                item.Genre = (string)m_BookGenrePicker.SelectedItem;
            }
            else
            {
                item = new VideoGame();
                (item as VideoGame).Genre = (string)m_VideoGameGenrePicker.SelectedItem;
                (item as VideoGame).Platform = StringToPlatform[(string)m_PlatformPicker.SelectedItem];
            }

            item.ImagesOfItem = new List<ItemFormServices.Image>(i_ImagesOfItem.Count);
            for (int i = 0; i < i_ImagesOfItem.Count; i++)
            {
                ItemFormServices.Image image = new ItemFormServices.Image
                {
                    BytesOfImage = string.Copy(i_ImagesOfItem[i])
                };

                item.ImagesOfItem.Add(image);
            }

            item.Name = ItemName.Text;
            item.IdCustomer = (Application.Current as App).UserId;
            item.Description = m_Description.Text;
            item.Condition = StringToItemCondition[(string)statePicker.SelectedItem];
            item.Views = 0;

            if (m_Item != null)
            {
                item.Id = m_Item.Id;
            }

            return item;
        }

        private async void takePhoto_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                if (i_ImagesOfItem.Count == 5)
                {
                    await DisplayAlert("הגבלת מ'ס תמונות לפריט", "באפשרותך להוסיף עד 5 תמונות לפריט!", "OK");
                    return;
                }

                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    CompressionQuality = 30,
                    CustomPhotoSize = 50,
                    DefaultCamera = CameraDevice.Rear,
                });

                if (photo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        photo.GetStream().CopyTo(memoryStream);
                        i_ImagesOfItem.Add(Convert.ToBase64String(memoryStream.ToArray()));

                        MR.Gestures.Image image = new MR.Gestures.Image()
                        {
                            Source = ImageSource.FromStream(() => photo.GetStream()),
                            HeightRequest = 180,
                            WidthRequest = 100,
                            Aspect = Aspect.AspectFill,
                        };

                        m_ImageToString[image] = Convert.ToBase64String(memoryStream.ToArray());
                        image.LongPressing += deleteImage_Button_LongPressing;
                        myImagesStackLayout.Children.Add(image);
                        image.Opacity = 0;

                        image.FadeTo(1, 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void pickPhoto_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }
               
                if (i_ImagesOfItem.Count == 5)
                {
                    await DisplayAlert("הגבלת מ'ס תמונות לפריט", "באפשרותך להוסיף עד 5 תמונות לפריט!", "OK");
                    return;
                }

                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    CompressionQuality = 30,
                    CustomPhotoSize = 50,
                });
                
                if (file == null)
                {
                    return;
                }

                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    i_ImagesOfItem.Add(Convert.ToBase64String(memoryStream.ToArray()));

                    MR.Gestures.Image image = new MR.Gestures.Image()
                    {

                        Source = ImageSource.FromStream(() => file.GetStream()),
                        HeightRequest = 180,
                        WidthRequest = 100,
                        Aspect = Aspect.AspectFill,
                    };

                    m_ImageToString[image] = Convert.ToBase64String(memoryStream.ToArray());
                    image.LongPressing += deleteImage_Button_LongPressing;

                    myImagesStackLayout.Children.Add(image);
                    image.Opacity = 0;
                    image.FadeTo(1, 1000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void deleteImage_Button_LongPressing(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("", "האם אתה בטוח שברצונך למחוק תמונה זו?", "כן", "לא");
            if (answer == false)
            {
                return;
            }

            Xamarin.Forms.Image currentImage = sender as Xamarin.Forms.Image;
            myImagesStackLayout.Children.Remove(currentImage);
            i_ImagesOfItem.Remove(m_ImageToString[currentImage]);
        }

        //----------Edit Item Page methods----------//
        public ItemFormPage(string i_edit, MyItemsPage i_page, Item i_Item)
        {
            m_Item = i_Item;
            InitializeComponent();
            initializeCommonPickers();
            BooksFormStackLayout = new StackLayout();
            VideoGamesFormStackLayout = new StackLayout();
            m_MyItemsPage = i_page;

            i_ImagesOfItem = new List<string>();
            typePicker.IsEnabled = false;

            m_Description = getDescriptionCustomEntry();
            m_DoneEditingStackLayout = getDoneEditingStackLayout();
            m_DeleteItemStackLayout = getDeleteItemStackLayout();

            Grid doneEditAndDeleteButtonsGrid = new Grid();
            doneEditAndDeleteButtonsGrid.Children.Add(m_DoneEditingStackLayout, 0, 0);
            doneEditAndDeleteButtonsGrid.Children.Add(m_DeleteItemStackLayout, 1, 0);

            doneEditAndDeleteButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            doneEditAndDeleteButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            if (m_Item is Book)
            {
                MakeBookForm(this, ref m_BookGenrePicker, ref m_AuthorEntry, ref m_NumberOfPagesEntry);
                mainStackLayout.Children.Add(BooksFormStackLayout);
                BooksFormStackLayout.Children.Add(m_Description);
                BooksFormStackLayout.Children.Add(doneEditAndDeleteButtonsGrid);
            }
            else if (m_Item is VideoGame)
            {
                MakeVideoGameForm(this, ref m_PlatformPicker, ref m_VideoGameGenrePicker);
                mainStackLayout.Children.Add(VideoGamesFormStackLayout);
                VideoGamesFormStackLayout.Children.Add(m_Description);
                VideoGamesFormStackLayout.Children.Add(doneEditAndDeleteButtonsGrid);
            }

            fillFormWithItemDetails(m_Item);
        }

        private void fillFormWithItemDetails(Item i_item)
        {
            typePicker.IsVisible = false;
            ItemName.Text = i_item.Name;

            ItemCondition condition = i_item.Condition;
            string state = StringToItemCondition.FirstOrDefault(x => x.Value == condition).Key;
            statePicker.SelectedItem = state;
            if (i_item is Book)
            {
                BookLable.IsVisible = true;
                m_AuthorEntry.Text = (i_item as Book).Author;
                m_NumberOfPagesEntry.Text = ((i_item as Book).Pages).ToString();

                m_BookGenrePicker.SelectedItem = i_item.Genre;
            }

            if (i_item is VideoGame)
            {
                videoGameLable.IsVisible = true;
                m_VideoGameGenrePicker.SelectedItem = m_VideoGameGenrePicker.SelectedItem;

                GamingPlatform platform = (i_item as VideoGame).Platform;
                string console = StringToPlatform.FirstOrDefault(x => x.Value == platform).Key;

                m_PlatformPicker.SelectedItem = console;
                m_VideoGameGenrePicker.SelectedItem = i_item.Genre;
            }
          
            m_Description.Text = i_item.Description;
            for (int i = 0; i < i_item.ImagesOfItem.Count; i++)
            {
                var byteArray = Convert.FromBase64String(i_item.ImagesOfItem[i].BytesOfImage);
                var imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));

                MR.Gestures.Image image = new MR.Gestures.Image()
                {
                    Source = imageSource,
                    HeightRequest = 180,
                    WidthRequest = 100,
                    Aspect = Aspect.AspectFill,

                };
          
                i_ImagesOfItem.Add(i_item.ImagesOfItem[i].BytesOfImage);
                m_ImageToString[image] = i_item.ImagesOfItem[i].BytesOfImage;
                image.LongPressing += deleteImage_Button_LongPressing;

                myImagesStackLayout.Children.Add(image);
                image.Opacity = 0;
                image.FadeTo(1, 2000);
            }
        }

        private StackLayout getDeleteItemStackLayout()
        {
            StackLayout result = new StackLayout();
            Label deleteItemLabel = (new Label()
            {
                Text = "מחק",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center

            });
         
            ImageButton deleteItemImageButton = new ImageButton()
            {
                Source = "delete_item.png",
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Center
            };

            deleteItemImageButton.Clicked += deleteItem_Button_clicked;
            result.Children.Add(deleteItemImageButton);
            result.Children.Add(deleteItemLabel);

            return result;
        }

        private StackLayout getDoneEditingStackLayout()
        {
            StackLayout result = new StackLayout();

            Label doneEditingLabel = (new Label()
            {
                Text = "השלם עריכה",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            });
            ImageButton doneEditingImageButton = new ImageButton()
            {
                Source = "edit_item.png",
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Center
            };

            doneEditingImageButton.Clicked += doneEditing_Button_clicked;
            result.Children.Add(doneEditingImageButton);
            result.Children.Add(doneEditingLabel);
            return result;
        }

        private async void deleteItem_Button_clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("", "האם אתה בטוח שברצונך למחוק פריט זה?", "כן", "לא");
            if (answer == false)
                return;
            if (m_DeleteItemButtonClickEnabled == false)
                return;
            m_DeleteItemButtonClickEnabled = false;
            m_MyItemsPage.removeItemFromStackLayout(m_Item);

            try
            {
                await ServerFacade.Items.DeleteItem(m_Item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאת מערכת", "נסה מאוחר יותר", "אישור");
            }

            await Navigation.PushAsync(new WaitingPage());
            await DisplayAlert("", "הפריט נמחק בהצלחה!", "OK");

            m_DeleteItemButtonClickEnabled = true;
            await Navigation.PopToRootAsync();
        }

        private async void doneEditing_Button_clicked(object sender, EventArgs e)
        {
            if (m_DoneEditingButtonClickEnabled == false)
            {
                return;
            }
            m_DoneEditingButtonClickEnabled = false;

            if (!allRequiredFieldsHadBeenFilled())
            {
                await DisplayAlert(" ישנם שדות חובה שלא מולאו", "מלא את כל השדות המופיעים ב (*)", "OK");
                m_DoneEditingButtonClickEnabled = true;
                return;
            }

            m_MyItemsPage.removeItemFromStackLayout(m_Item);
            Item item = getItem();
            try
            {
                await ServerFacade.Items.EditItem(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאת מערכת", "נסה מאוחר יותר", "אישור");
            }
            await Navigation.PushAsync(new WaitingPage());
            m_MyItemsPage.addItemToStackLayout(item);
            await DisplayAlert("", "הפריט נערך בהצלחה!", "OK");
            m_DoneEditingButtonClickEnabled = true;
            await Navigation.PopToRootAsync();
        }


        //----------Add Item Page methods----------//
        public ItemFormPage(string i_add, MyItemsPage i_page)
        {
            InitializeComponent();
            initializeCommonPickers();
            BooksFormStackLayout = new StackLayout();
            VideoGamesFormStackLayout = new StackLayout();
            i_ImagesOfItem = new List<string>();
            m_MyItemsPage = i_page;
        }

        private Button getOkButton()
        {
            Button okButton = (new Button()
            {
                Text = "אישור",
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.SpringGreen

            });
            
            okButton.Clicked += ok_Button_clicked;

            return okButton;
        }

        private void TypeOfItemPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)typePicker.SelectedItem == "ספר")
            {
                if (m_BookFormHadBuilt == false)
                {
                    MakeBookForm(this, ref m_BookGenrePicker, ref m_AuthorEntry, ref m_NumberOfPagesEntry);
                    m_Description = getDescriptionCustomEntry();
                    m_OkButton = getOkButton();
                    BooksFormStackLayout.Children.Add(m_Description);
                    BooksFormStackLayout.Children.Add(m_OkButton);

                    m_BookFormHadBuilt = true;
                }
             
                mainStackLayout.Children.Remove(VideoGamesFormStackLayout);
                mainStackLayout.Children.Add(BooksFormStackLayout);
            }
            else if ((string)typePicker.SelectedItem == "משחק וידאו")
            {
                if (m_VideoGameFormHadBuilt == false)
                {
                    MakeVideoGameForm(this, ref m_PlatformPicker, ref m_VideoGameGenrePicker);
                    m_Description = getDescriptionCustomEntry();
                    m_OkButton = getOkButton();
                    VideoGamesFormStackLayout.Children.Add(m_Description);
                    VideoGamesFormStackLayout.Children.Add(m_OkButton);
                    m_VideoGameFormHadBuilt = true;
                }
             
                mainStackLayout.Children.Remove(BooksFormStackLayout);
                mainStackLayout.Children.Add(VideoGamesFormStackLayout);
            }
        }

        private async void ok_Button_clicked(object sender, EventArgs e)
        {
          
            if (m_OkButtonClickEnabled == false)
            {
                return;
            }
            m_OkButtonClickEnabled = false;

            if (!allRequiredFieldsHadBeenFilled())
            {
                await DisplayAlert(" ישנם שדות חובה שלא מולאו", "מלא את כל השדות המופיעים ב (*)", "OK");
                m_OkButtonClickEnabled = true;
                return;
            }

            Item item = getItem();
            try
            {
                await ServerFacade.Items.AddItem(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאת מערכת", "נסה מאוחר יותר", "אישור");
            }
          
            int currentIdCustomer = (Application.Current as App).UserId;
            await m_MyItemsPage.DisplayItemsOfUser(currentIdCustomer, "Last Uploaded");
            await DisplayAlert("", "הפריט נוסף בהצלחה!", "OK");
            m_OkButtonClickEnabled = true;
            await Navigation.PopToRootAsync();
        }
    }
}