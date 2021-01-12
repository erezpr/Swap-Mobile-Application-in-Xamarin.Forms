using Swap.Converters;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoContentView : ContentView
    {
        public static readonly BindableProperty TitleProperty =
                   BindableProperty.Create(nameof(Title), typeof(string), typeof(InfoContentView), "");
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty ImageSourceProperty =
                  BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(InfoContentView), "");
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
                  BindableProperty.Create(nameof(Text), typeof(string), typeof(InfoContentView), "");
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public InfoContentView()
        {
            BindingContext = this;
            InitializeComponent();
        }
    }
}