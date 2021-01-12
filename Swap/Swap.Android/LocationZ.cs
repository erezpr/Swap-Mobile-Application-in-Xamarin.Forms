using Android.Content;
using Android.Locations;
using Swap.Droid;
using Swap.Services;

[assembly: Xamarin.Forms.Dependency(typeof(LocationZ))]
namespace Swap.Droid
{
    public class LocationZ : ILocationService
    {
        public void OpenSettings()
        {
            LocationManager LM = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);

            if (LM.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                Context ctx = Android.App.Application.Context;

                Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                intent.SetFlags(ActivityFlags.NewTask);

                ctx.StartActivity(intent);
                throw new GpsNotAvailableException();
            }
        }
    }
}

