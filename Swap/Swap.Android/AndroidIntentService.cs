using Android.Content;
using Android.Graphics;
using Swap.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: Dependency(typeof(Swap.Droid.AndroidIntentService))]
namespace Swap.Droid
{
    public class AndroidIntentService : IIntentService
    {
        string FilePath;
        Java.IO.File dir;
        public bool Email(string mail, ImageSource mBitmap)
        {

            saveImageLocally(mBitmap);

            string localAbsoluteFilePath = FilePath;

            if (localAbsoluteFilePath != null && localAbsoluteFilePath != "")
            {

                Intent shareIntent = new Intent(Intent.ActionSend);

                Android.Net.Uri phototUri = Android.Net.Uri.Parse(localAbsoluteFilePath);

                using (Java.IO.File file = new Java.IO.File(phototUri.Path))
                {

                    //Log.d(TAG, "file path: " +file.LocalPath());

                    if (file.Exists())
                    {
                        // file create success

                    }
                    else
                    {
                        // file create fail
                    }
                }
                shareIntent.SetData(phototUri);
                shareIntent.SetType("image/png");
                shareIntent.PutExtra(Intent.ExtraStream, phototUri);
                
                Forms.Context.StartActivity(Intent.CreateChooser(shareIntent, "Share Via"));
            }




            return true;
        }

        private void createDirectoryForPictures()
        {
            dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }
        }

        private async void saveImageLocally(ImageSource _bitmap)

        {

            //Java.IO.File outputDir = Android.OS.Environment.GetExternalStoragePublicDirectory("Downloads");
            //Java.IO.File outputFile = null;

            createDirectoryForPictures();
            try
            {
                //outputFile = Java.IO.File.CreateTempFile("tmp", ".png", dir);
                //outputFile.Dispose();
                
            }
            catch (IOException)
            {
                // handle exception
            }
            Bitmap bitmap = await getBitmap(_bitmap);


            var finalStream = new MemoryStream();

            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 25, finalStream);
            //            bitmap = null;
            finalStream.Position = 0;

            var path2 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filename2 = System.IO.Path.Combine(path2, "teste.png");
            using (FileStream fileStream = System.IO.File.Create(filename2))
            {
                finalStream.Seek(0, SeekOrigin.Begin);
                finalStream.CopyTo(fileStream);
                fileStream.Close();

                finalStream.Dispose();
                //stream.Dispose ();
                fileStream.Dispose();
                GC.Collect();
            }
            FilePath = filename2;
        }

        private Task<Bitmap> getBitmap(ImageSource image)
        {
            return getImageFromImageSource(image, Forms.Context);
        }

        private async Task<Bitmap> getImageFromImageSource(ImageSource imageSource, Context context)
        {
            IImageSourceHandler handler;

            if (imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if (imageSource is StreamImageSource)
            {
                handler = new StreamImagesourceHandler(); // sic
            }
            else if (imageSource is UriImageSource)
            {
                handler = new ImageLoaderSourceHandler(); // sic
            }
            else
            {
                throw new NotImplementedException();
            }

            return await handler.LoadImageAsync(imageSource, context);
        }
    }
}


