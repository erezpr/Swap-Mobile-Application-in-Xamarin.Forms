using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using ContextMenu.Droid;
using FFImageLoading.Forms.Platform;
using Firebase.Iid;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.CurrentActivity;
using System.Threading.Tasks;
using Android.Views;

namespace Swap.Droid
{
    [Activity(Label = "Swap", Icon = "@mipmap/icon", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "MainActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //if (Intent.Extras != null)
            //{
            //    foreach (string item in Intent.Extras.KeySet())
            //    {
            //        string value = Intent.Extras.GetString(item);
            //    }
            //}
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Window.AddFlags(WindowManagerFlags.Fullscreen); 
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
            global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ImageCircleRenderer.Init();
            //CachedImageRenderer.InitImageViewHandler();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle
            CachedImageRenderer.Init(enableFastRenderer: true);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            ContextMenuViewRenderer.Preserve();
            Task.Run(() => {
                var refreshedToken = FirebaseInstanceId.Instance.Token;
                (Xamarin.Forms.Application.Current as App).FireBaseToken = refreshedToken;
                Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            });
            LoadApplication(new App());
        }

        //private bool IsPlayServicesAvailable()
        //{
        //    int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        //    if (resultCode != ConnectionResult.Success)
        //    {
        //        if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
        //        {
        //            Log.Debug("error", GoogleApiAvailability.Instance.GetErrorString(resultCode));
        //        }
        //        else
        //        {
        //            Log.Debug("error", "This device is not supported");
        //            Finish();
        //        }
        //        return false;
        //    }
        //    else
        //    {
        //        Log.Debug("success", "Google play services is available");
        //        return true;
        //    }
        //}

        //private void CreateNotificationChannel()
        //{
        //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        //    {
        //        // Notification channels are new in API 26 (and not a part of the
        //        // support library). There is no need to create a notification
        //        // channel on older versions of Android.
        //        return;
        //    }

        //    var channel = new NotificationChannel(CHANNEL_ID,
        //                                          "FCM Notifications",
        //                                          NotificationImportance.Default)
        //    {

        //        Description = "Firebase Cloud Messages appear in this channel"
        //    };

        //    var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
        //    notificationManager.CreateNotificationChannel(channel);
        //}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}