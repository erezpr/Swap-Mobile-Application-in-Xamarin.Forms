using System;
using System.IO;
using Xamarin.Forms;

namespace Swap.Converters
{
    class BitMapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource retSource = null;
            if (value != null)
            {
                if (value != null)
                {
                    byte[] decodedByteArray = System.Convert.FromBase64String(value.ToString());
                    retSource = ImageSource.FromStream(() => new MemoryStream(decodedByteArray));
                }
            }
            
            return retSource;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}