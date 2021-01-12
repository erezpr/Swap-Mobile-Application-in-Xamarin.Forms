using Swap.Enums;
using System;
using Xamarin.Forms;

namespace Swap.Converters
{
    class TradeStatusToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((TradeStatus)value)
            {
                case TradeStatus.WaitingForAction:
                    {
                        return "ממתין להחלטתך";
                    }
                case TradeStatus.Rejected:
                    {
                        return "ההצעה נדחתה";
                    }
                case TradeStatus.Accepted:
                    {
                        return "יש התאמה";
                    }
                default:
                    {
                        return "";
                    }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}