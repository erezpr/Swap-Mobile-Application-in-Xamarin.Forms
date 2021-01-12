using Android.Content;
using Android.Graphics.Drawables;
using Swap.CustomViews;
using Swap.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ErezCustomEntry), typeof(ErezCustomEntryRenderer))]
namespace Swap.Droid
{
    public class ErezCustomEntryRenderer : EntryRenderer
    {
        public ErezCustomEntryRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.Background = new ColorDrawable(Android.Graphics.Color.White);
                Control.Gravity = Android.Views.GravityFlags.Right;
            }
        }
    }
}