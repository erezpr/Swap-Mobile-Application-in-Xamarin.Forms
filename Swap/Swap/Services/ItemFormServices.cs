using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swap.CustomViews;
using Swap.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Swap.Services.ItemFormServices;

namespace Swap.Services
{
    public static class ItemFormServices
    {
        public enum GamingPlatform
        {
            PC = 0,
            Xbox360 = 1,
            XboxOne = 2,
            Playstation3 = 3,
            Playstation4 = 4,
            N3DS = 5,
            Switch = 6,
            Other = 7
        }

        public enum ItemCondition
        {
            New = 0,
            NewOther = 1,
            OpenBox = 2,
            Mint = 3,
            Used = 4,
            PartsOrNotWorking = 5
        }

        public class Image
        {
            public string BytesOfImage { get; set; }
        }

        [JsonConverter(typeof(ItemJsonConverter))]
        public class Item
        {
            public int Id;
            public string Name;
            public int IdCustomer;
            public string Genre;
            public string Description;
            public ItemCondition Condition;
            public int Views;
            public DateTime UploadDate;
            public List<Image> ImagesOfItem;
        }

        internal class Book : Item
        {
            public string Author { get; set; }
            public short Pages { get; set; }

            internal Book() { }
        }

        internal class VideoGame : Item
        {
            public GamingPlatform Platform { get; set; }

            internal VideoGame() { }
        }

        public static List<string> state = new List<string>()
        {
            "מצוין",
            "כמו חדש",
            "טוב",
            "בינוני",
            "משומש",
            "איכות ירודה"
        };

        public static List<string> typeOfItem = new List<string>()
        {
            "ספר",
            "משחק וידאו"
        };

        public static List<string> typeOfBook = new List<string>()
        {
            "רומן",
            "הרפתקאות",
            "מדע בדיוני",
            "פנטזיה",
            "ילדים",
            "מתח",
            "לימוד",
            "קומיקס",
            "עיון",
            "הדרכה",
            "שירה",
            "יומן ואוטוביוגרפיה",
            "ספרות מקצועית",
            "אחר"
        };

        public static List<string> videoGameGenre = new List<string>()
        {
            "פעולה",
            "הרפתקאות",
            "ספורט",
            "מכות",
            "אסטרטגיה",
            "תפקידים",
            "מירוצים",
            "חשיבה",
            "אחר"
        };

        public static List<string> platform = new List<string>()
        {
            "PC",
            "Xbox 360",
            "Xbox One",
            "PlayStation 3",
            "PlayStation 4",
            "Nintendo 3DS",
            "Nintendo Switch",
            "אחר"

        };

        public static Frame SetNewFrame(string i_Type, Page i_Page, params View[] i_ChildrenObjects)
        {
            Frame result = new Frame();
            StackLayout SL = new StackLayout();

            SL.Children.Add(i_ChildrenObjects[0]);
            SL.Children.Add(i_ChildrenObjects[1]);

            result.Content = SL;

            if (i_Type == "Book")
            {
                if (i_Page is ItemFormPage)
                {
                    (i_Page as ItemFormPage).BooksFormStackLayout.Children.Add(result);

                }
                else if (i_Page is SearchPage)
                {
                    (i_Page as SearchPage).m_BooksStackLayout.Children.Add(result);
                }
            }

            else if (i_Type == "Video Game")
            {
                if (i_Page is ItemFormPage)
                {
                    (i_Page as ItemFormPage).VideoGamesFormStackLayout.Children.Add(result);

                }
                else if (i_Page is SearchPage)
                {
                    (i_Page as SearchPage).m_VideoGamesStackLayout.Children.Add(result);
                }
            }

            return result;
        }
        public static Label SetNewLabel(string i_Text)
        {
            Label result = new Label()
            {

                Text = i_Text,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
            };

            return result;
        }

        internal readonly static Dictionary<string, GamingPlatform> StringToPlatform = new Dictionary<string, GamingPlatform>()
        {
            { "PC", GamingPlatform.PC },
            { "Xbox 360",GamingPlatform.Xbox360 },
            { "Xbox One", GamingPlatform.XboxOne},
            { "PlayStation 3",GamingPlatform.Playstation3 },
            { "PlayStation 4", GamingPlatform.Playstation4},
            { "Nintendo 3DS", GamingPlatform.N3DS},
            { "Nintendo Switch", GamingPlatform.Switch },
            { "אחר",GamingPlatform.Other }
        };

        internal readonly static Dictionary<string, ItemCondition> StringToItemCondition = new Dictionary<string, ItemCondition>()
        {
            { "מצוין", ItemCondition.New},
            { "כמו חדש", ItemCondition.NewOther},
            { "טוב", ItemCondition.OpenBox },
            { "בינוני", ItemCondition.Mint},
            { "משומש", ItemCondition.Used},
            { "איכות ירודה", ItemCondition.PartsOrNotWorking}
        };

        internal readonly static Dictionary<GamingPlatform, string> PlatformToString = new Dictionary<GamingPlatform, string>()
        {
            { GamingPlatform.PC , "PC"},
            { GamingPlatform.Xbox360 ,"Xbox 360"},
            { GamingPlatform.XboxOne, "Xbox One"},
            { GamingPlatform.Playstation3 ,"PlayStation 3"},
            { GamingPlatform.Playstation4, "PlayStation 4"},
            { GamingPlatform.N3DS, "Nintendo 3DS"},
            { GamingPlatform.Switch , "Nintendo Switch"},
            { GamingPlatform.Other ,"Other"}
        };

        internal readonly static Dictionary<ItemCondition, string> ItemConditionToString = new Dictionary<ItemCondition, string>()
        {
            { ItemCondition.New, "מצויין"},
            { ItemCondition.NewOther, "כמו חדש"},
            { ItemCondition.OpenBox , "טוב"},
            { ItemCondition.Mint, "בינוני"},
            { ItemCondition.Used, "משומש"},
            { ItemCondition.PartsOrNotWorking, "איכות ירודה"}
        };

        public static ImageSource GetImageSource(Item i_Item, int i_Index)
        {
            Image mainImage = null;

            if (i_Item.ImagesOfItem.Count > i_Index)
            {
                mainImage = i_Item.ImagesOfItem[i_Index];

                byte[] byteArray = Convert.FromBase64String(mainImage.BytesOfImage);
                return ImageSource.FromStream(() =>
                    new MemoryStream(byteArray)
                );
            }

            return null;
        }

        public static async Task<List<Item>> GetItems(int i_Id)
        {
            HttpClient client = new HttpClient();
            HttpMethod httpMethod = HttpMethod.Get;
            string data = string.Format("id={0}", i_Id);
            string Token = "Bearer " + (Application.Current as App).Token;
            string UrlAllItems = "http://Vmedu184.mtacloud.co.il/item/getItems" + "?" + data;
            HttpRequestMessage request = new HttpRequestMessage() { Method = httpMethod };

            request.RequestUri = new Uri(UrlAllItems);

            request.Headers.Add("Authorization", Token);
            HttpResponseMessage httpResponse = await client.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Item>>(responseContent);
        }
        //-----book methods-----//

        public static CustomPicker SetTypeFrame(Page i_Page)
        {
            CustomPicker typeOfBookPicker = new CustomPicker()
            {
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 45
            };

            if (i_Page is SearchPage)
            {
                typeOfBookPicker.Items.Add("הכל");
                typeOfBookPicker.SelectedItem = "הכל";
            }

            foreach (string choice in typeOfBook)
            {
                typeOfBookPicker.Items.Add(choice);
            }

            Label typeOfBookLable = SetNewLabel("סוג הספר / ג'אנר ");
            if (i_Page is ItemFormPage)
            {
                typeOfBookLable.Text += "(*)";
            }

            Frame typeOfBookFrame = SetNewFrame("Book", i_Page, typeOfBookLable, typeOfBookPicker);
            return typeOfBookPicker;
        }
        public static CustomEntry SetAuthorFrame(Page i_Page)
        {
            CustomEntry authorEntry = new CustomEntry()
            {
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 45
            };

            Label authorLable = SetNewLabel("מחבר");
            Frame authorFrame = SetNewFrame("Book", i_Page, authorLable, authorEntry);
            return authorEntry;
        }
        public static CustomEntry SetNumberOfPagesFrame(Page i_Page)
        {
            CustomEntry numberOfPagesEntry = new CustomEntry()
            {
                Keyboard = Keyboard.Numeric,
                HorizontalTextAlignment = TextAlignment.End,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 45
            };

            Label numberOfPagesLable = SetNewLabel("מספר עמודים");
            Frame numberOfPagesFrame = SetNewFrame("Book", i_Page, numberOfPagesLable, numberOfPagesEntry);

            return numberOfPagesEntry;
        }

        public static void MakeBookForm(Page i_Page, ref CustomPicker i_TypePicker, ref CustomEntry i_AuthorEntry, ref CustomEntry i_NumberOfPages)
        {
            i_TypePicker = SetTypeFrame(i_Page);
            i_AuthorEntry = SetAuthorFrame(i_Page);
            if (i_Page is ItemFormPage)
            {
                i_NumberOfPages = SetNumberOfPagesFrame(i_Page);
            }
        }

        //-----video game methods-----//
        public static CustomPicker SetPlatformFrame(Page i_Page)
        {
            CustomPicker platformPicker = new CustomPicker()
            {
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 45
            };

            if (i_Page is SearchPage)
            {
                platformPicker.Items.Add("הכל");
                platformPicker.SelectedItem = "הכל";

            }

            foreach (string choice in platform)
            {
                platformPicker.Items.Add(choice);
            }

            Label platformLable = SetNewLabel("פלטפורמה ");
            if (i_Page is ItemFormPage)
            {
                platformLable.Text += "(*)";

            }

            Frame videoGameplatformFrame = SetNewFrame("Video Game", i_Page, platformLable, platformPicker);
            return platformPicker;
        }

        public static CustomPicker SetGenreFrame(Page i_Page)
        {
            CustomPicker genrePicker = new CustomPicker()
            {
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 45
            };

            if (i_Page is SearchPage)
            {
                genrePicker.Items.Add("הכל");
                genrePicker.SelectedItem = "הכל";

            }
            foreach (string choice in videoGameGenre)
            {
                genrePicker.Items.Add(choice);
            }

            Label genreLable = SetNewLabel("ג'אנר ");
            if (i_Page is ItemFormPage)
            {
                genreLable.Text += "(*)";
            }
            Frame videoGameGenreFrame = SetNewFrame("Video Game", i_Page, genreLable, genrePicker);
            return genrePicker;
        }
        public static void MakeVideoGameForm(Page i_page, ref CustomPicker i_platformPicker, ref CustomPicker i_genrePicker)
        {
            i_platformPicker = SetPlatformFrame(i_page);
            i_genrePicker = SetGenreFrame(i_page);
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected abstract T Create(Type objectType, JObject json);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("Reader is null");
            if (serializer == null)
                throw new ArgumentNullException("Serializer is null");
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject json = JObject.Load(reader);
            T target = Create(objectType, json);
            serializer.Populate(json.CreateReader(), target);
            return target;
        }
    }

    public class ItemJsonConverter : JsonCreationConverter<Item>
    {
        protected override Item Create(Type objectType, JObject json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("Json object is null");
            }

            if (json["platform"] != null)
            {
                return new VideoGame();
            }
            else if (json["author"] != null || json["pages"] != null)
            {
                return new Book();
            }
            else
            {
                return new Item();
            }
        }
    }
}