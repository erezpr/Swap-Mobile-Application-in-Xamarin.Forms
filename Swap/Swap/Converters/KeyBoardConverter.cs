using System;
using System.Globalization;
using Xamarin.Forms;

namespace Swap.Converters
{
    class KeyBoardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "Email":
                    {
                        return Keyboard.Email;
                    }
                case "Numeric":
                    {
                        return Keyboard.Numeric;
                    }
                default:
                    {
                        return Keyboard.Default;
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}