using Xamarin.Forms;

namespace Swap.Services
{
    public interface IIntentService
    {
        bool Email(string mail, ImageSource mBitmap);
    }
}