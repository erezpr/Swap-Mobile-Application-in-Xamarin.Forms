using FFImageLoading.Forms;
using System;
using Xamarin.Forms;

namespace Swap.CustomViews
{
    public class CustomCacheKeyFactory : ICacheKeyFactory
    {
        public CustomCacheKeyFactory() { }
        public string GetKey(ImageSource imageSource, object bindingContext)
        {
            if (imageSource == null)
                return null;

            string itemSuffix = string.Empty;

            UriImageSource uriImageSource = imageSource as UriImageSource;
            if (uriImageSource != null)
                return string.Format("{0}+myCustomUriSuffix+{1}", uriImageSource.Uri, itemSuffix);

            FileImageSource fileImageSource = imageSource as FileImageSource;
            if (fileImageSource != null)
                return string.Format("{0}+myCustomFileSuffix+{1}", fileImageSource.File, itemSuffix);

            StreamImageSource streamImageSource = imageSource as StreamImageSource;
            if (streamImageSource != null)
                return string.Format("{0}+myCustomStreamSuffix+{1}", streamImageSource.Stream.GetHashCode(), itemSuffix);

            throw new NotImplementedException("ImageSource type not supported");
        }
    }
}