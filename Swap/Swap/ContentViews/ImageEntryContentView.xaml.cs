using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageEntryContentView : ContentView
    {
        public static readonly BindableProperty TitleProperty =
                   BindableProperty.Create(nameof(Title), typeof(string), typeof(ImageEntryContentView), "");
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty PlaceholderProperty =
                   BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ImageEntryContentView), "");
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
                   BindableProperty.Create(nameof(Text), typeof(string), typeof(ImageEntryContentView), "");
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty ImageSourceProperty =
                  BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(ImageEntryContentView), "");
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly BindableProperty IsRequiredProperty =
                   BindableProperty.Create(nameof(IsRequired), typeof(bool), typeof(ImageEntryContentView), false);
        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }

        public static readonly BindableProperty IsPasswordProperty =
                  BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(ImageEntryContentView), false);
        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        public static readonly BindableProperty KeyboardProperty =
                  BindableProperty.Create(nameof(Keyboard), typeof(string), typeof(ImageEntryContentView), "");
        public string Keyboard
        {
            get { return (string)GetValue(KeyboardProperty); }
            set { SetValue(KeyboardProperty, value); }
        }

        public ImageEntryContentView()
        {
            BindingContext = this;
            InitializeComponent();
        }
    }
}
