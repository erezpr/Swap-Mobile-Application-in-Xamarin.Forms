using Swap.Enums;
using System.Text.RegularExpressions;

namespace Swap.Services
{
    public static class StringValidationService
    {
        private static Regex regex;
        private static Match match;

        public static bool IsValid(string i_StringToValidate, ValidationType i_ValidationType)
        {
            switch (i_ValidationType)
            {
                case ValidationType.Email:
                    {
                        regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        match = regex.Match(i_StringToValidate);
                    }
                    break;
                case ValidationType.Password:
                    {
                        regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
                        match = regex.Match(i_StringToValidate);
                    }
                    break;
                case ValidationType.Name:
                    {
                        regex = new Regex(@"");
                        match = regex.Match(i_StringToValidate);
                    }
                    break;
                case ValidationType.PhoneNumber:
                    {
                        regex = new Regex(@"^\+?(972|0)(\-)?0?(([23489]{1}\d{7})|[5]{1}\d{8})$");
                        match = regex.Match(i_StringToValidate);
                    }
                    break;
            }
            return match.Success;
        }
    }
}