using FFImageLoading.Forms;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageContentView : ContentView
    {
        public static readonly BindableProperty ImageSourceProperty =
          BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(ImageContentView));
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public ImageContentView()
        {
            InitializeComponent();
        }

        public ImageContentView(ImageSource i_ImageSource, string description, DateTime updateTime)
        {
            InitializeComponent();

            BindingContext = this;
        }
    }

}