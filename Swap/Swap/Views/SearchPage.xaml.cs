using Swap.CustomViews;
using Swap.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        private App app = Application.Current as App;
        public StackLayout m_BooksStackLayout { get; set; }
        public StackLayout m_VideoGamesStackLayout { get; set; }
        private bool m_BookFormHadBuilt = false;
        private bool m_VideoGameFormHadBuilt = false;
        private CustomPicker m_BookTypePicker, m_PlatformPicker, m_GenrePicker;
        private CustomEntry m_NumberOfPagesEntry, m_AuthorEntry;
        private bool m_SearchButtonClickEnabled = true;

        public enum ItemType
        {
            Item,
            Book,
            Videogame
        };

        internal readonly static Dictionary<string, ItemType> StringToItemType = new Dictionary<string, ItemType>()
        {
            { "ספר", ItemType.Book },
            { "משחק וידאו",ItemType.Videogame}
        };

        public SearchPage()
        {
            InitializeComponent();
            initializeCommonPickers();
            m_BooksStackLayout = new StackLayout();
            m_VideoGamesStackLayout = new StackLayout();
        }

        private void initializeCommonPickers()
        {
            statePicker.Items.Add("הכל");
            statePicker.SelectedItem = "הכל";
            foreach (string choice in ItemFormServices.state)
            {
                statePicker.Items.Add(choice);
            }

            typePicker.Items.Add("הכל");
            typePicker.SelectedItem = "הכל";

            foreach (string choice in ItemFormServices.typeOfItem)
            {
                typePicker.Items.Add(choice);
            }
        }

        private void TypeOfItemPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)typePicker.SelectedItem == "הכל")
            {
                if (m_BooksStackLayout != null)
                {
                    mainStackLayout.Children.Remove(m_BooksStackLayout);
                }

                if (m_VideoGamesStackLayout != null)
                {
                    mainStackLayout.Children.Remove(m_VideoGamesStackLayout);
                }
            }
            else if ((string)typePicker.SelectedItem == "ספר")
            {
                if (m_BookFormHadBuilt == false)
                {
                    ItemFormServices.MakeBookForm(this, ref m_BookTypePicker, ref m_AuthorEntry, ref m_NumberOfPagesEntry);
                    m_BookFormHadBuilt = true;
                }

                mainStackLayout.Children.Remove(m_VideoGamesStackLayout);
                mainStackLayout.Children.Add(m_BooksStackLayout);
            }
            else if ((string)typePicker.SelectedItem == "משחק וידאו")
            {
                if (m_VideoGameFormHadBuilt == false)
                {
                    ItemFormServices.MakeVideoGameForm(this, ref m_PlatformPicker, ref m_GenrePicker);
                    m_VideoGameFormHadBuilt = true;
                }

                mainStackLayout.Children.Remove(m_BooksStackLayout);
                mainStackLayout.Children.Add(m_VideoGamesStackLayout);
            }
        }

        private string getParametersForSearch()
        {
            string result = null;
            if (radiusSwitch.IsToggled == true)
            {
                result += string.Format("rad={0}&", RadiusSlider.Value);
            }
            else if (citySwitch.IsToggled == true)
            {
                result += string.Format("city={0}&", cityEntry.Text);
            }

            if (ItemName.Text != null)
            {
                result += string.Format("name={0}&", ItemName.Text);
            }

            if (statePicker.SelectedItem != null && (string)statePicker.SelectedItem != "הכל")
            {
                result += string.Format("con={0}&", (int)ItemFormServices.StringToItemCondition[(string)statePicker.SelectedItem]);
            }

            if (typePicker.SelectedItem != null && (string)typePicker.SelectedItem != "הכל")
            {
                result += string.Format("t={0}&", (int)StringToItemType[(string)typePicker.SelectedItem]);
            }

            if ((string)typePicker.SelectedItem == "ספר")
            {
                if (m_BookTypePicker.SelectedItem != null && (string)m_BookTypePicker.SelectedItem != "הכל")
                {
                    result += string.Format("gen={0}&", (string)m_BookTypePicker.SelectedItem);
                }

                if (m_AuthorEntry.Text != null)
                {
                    result += string.Format("au={0}&", m_AuthorEntry.Text);
                }
            }

            if ((string)typePicker.SelectedItem == "משחק וידאו")
            {
                if (m_GenrePicker.SelectedItem != null && (string)m_GenrePicker.SelectedItem != "הכל")
                {
                    result += string.Format("gen={0}&", (string)m_GenrePicker.SelectedItem);
                }

                if (m_PlatformPicker.SelectedItem != null && (string)m_PlatformPicker.SelectedItem != "הכל")
                {
                    result += string.Format("plat={0}&", (int)ItemFormServices.StringToPlatform[(string)m_PlatformPicker.SelectedItem]);
                }
            }

            if (result != null)
            {
                if (result.EndsWith("&") == true)
                {
                    result = result.Remove(result.Length - 1);
                }
            }

            return result;
        }

        async void search_Button_Clicked(object sender, EventArgs e)
        {
            if (m_SearchButtonClickEnabled == false)
                return;

            m_SearchButtonClickEnabled = false;

            string parameters = getParametersForSearch();
            try
            {
                if (string.IsNullOrWhiteSpace(app.Token) == true)
                {
                    await Shell.Current.DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "אישור");
                    return;
                }
                List<ItemFormServices.Item> items = await ServerFacade.Items.SearchItemsAsync(parameters);
                await Navigation.PushAsync(new SearchResultsPage(items));
                m_SearchButtonClickEnabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                m_SearchButtonClickEnabled = true;
            }
        }

        private void radius_Switch_Toggled_(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (citySwitch.IsToggled)
                {
                    citySwitch.IsToggled = false;
                }
            }
        }

        private void city_Switch_Toggled_(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (radiusSwitch.IsToggled)
                {
                    radiusSwitch.IsToggled = false;
                }
            }
        }
    }
}