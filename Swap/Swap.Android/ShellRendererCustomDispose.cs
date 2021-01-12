using Android.Content;
using Swap.Droid;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Shell), typeof(ShellRendererCustomDispose))]
namespace Swap.Droid
{
    public class ShellRendererCustomDispose : ShellRenderer
    {
        bool _disposed;

        public ShellRendererCustomDispose(Context context)
            : base(context)
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Element.PropertyChanged -= OnElementPropertyChanged;
                Element.SizeChanged -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), this, "OnElementSizeChanged"); // OnElementSizeChanged is private, so use reflection
            }

            _disposed = true;
        }
    }
}