using Swap.Enums;
using System;
using Xamarin.Forms;

namespace Swap.Converters
{
    class TradeStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((TradeStatus)value)
            {
                case TradeStatus.WaitingForAction:
                    {
                        return Color.Default;
                    }
                case TradeStatus.Rejected:
                    {
                        return Color.Red;
                    }
                case TradeStatus.Accepted:
                    {
                        return Color.Green;
                    }
                default:
                    {
                        return Color.Default;
                    }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}