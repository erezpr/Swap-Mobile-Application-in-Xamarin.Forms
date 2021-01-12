using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;
using System;

namespace Swap.Droid.PushNotificationServices
{
    [Service]
    [IntentFilter(new[] {
        "com.google.firebase.MESSAGING_EVENT"
    })]
    class MyFireMessagingService : FirebaseMessagingService
    {
        private const string NOTIFICATION_CHANNEL_ID = "";
        private const string TAG = "MyFireMessagingService";

        public override void OnNewToken(string refreshedToken)
        {
            base.OnNewToken(refreshedToken);
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            App app = Xamarin.Forms.Application.Current as App;
            app.FireBaseToken = refreshedToken;
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            App app = Xamarin.Forms.Application.Current as App;
            app.IsUserHaveNewNotificationMessage = true;
            base.OnMessageReceived(message);
            RemoteMessage.Notification notification = message.GetNotification();
            sendNotificaiton(notification.Title, notification.Body);
        }

        private void sendNotificaiton(string title, string body)
        {
            NotificationManager notificationManager =
                (NotificationManager)GetSystemService(Context.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(
                    NOTIFICATION_CHANNEL_ID, "Notification Channel", NotificationImportance.Default)
                {
                    Description = "Swap Channel",
                    LightColor = Color.Red
                };
                notificationChannel.EnableLights(true);
                notificationManager.CreateNotificationChannel(notificationChannel);
            }

            using (NotificationCompat.Builder notificationBuilder =
                new NotificationCompat.Builder(context: this, channelId: NOTIFICATION_CHANNEL_ID))
            {
                notificationBuilder.SetAutoCancel(true).SetDefaults(-1)
                    .SetWhen(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                    .SetContentTitle(title)
                    .SetContentText(body)
                    .SetSmallIcon(Resource.Drawable.ic_like)
                    .SetContentInfo("Info");
                notificationManager.Notify(new Random().Next(), notificationBuilder.Build());
            }
        }
    }
}