using Swap.Enums;
using System;
using Xamarin.Forms;

namespace Swap.Converters
{
    class TradeStatusToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result;

            switch ((TradeStatus)value)
            {
                case TradeStatus.WaitingForAction:
                    {
                        result = true;
                    }
                    break;
                case TradeStatus.Rejected:
                    {
                        result = false;
                    }
                    break;
                case TradeStatus.Accepted:
                    {
                        result = false;
                    }
                    break;
                default:
                    {
                        result = true;
                    }
                    break;
            }

            if (parameter.ToString() == "false")
            {
                return result;
            }
            else
            {
                return !result;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}