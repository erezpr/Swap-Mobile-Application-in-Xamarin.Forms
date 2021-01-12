using Plugin.Media;
using Plugin.Media.Abstractions;
using Swap.Enums;
using Swap.Services;
using Swap.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private readonly App app = Application.Current as App;

        public ProfileViewModel ViewModel
        {
            get { return BindingContext as ProfileViewModel; }
            set { BindingContext = value; }
        }

        public ProfilePage()
        {
            ViewModel = new ProfileViewModel();
            ViewModel.ModeChanged += changeModeAnimationAsync;
            app.ProfileViewModel = ViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.UpdateInfo();
            if (string.IsNullOrWhiteSpace(app.Token) == false)
            {
                if (ViewModel.IsUserLogin == false)
                {
                    ViewModel.IsUserLogin = true;
                    ViewModel.Mode = ProfileMode.Details;
                }
            }
            else
            {
                ViewModel.IsUserLogin = false;
                ViewModel.ImageSource = ImageSource.FromResource("Swap.Images.profile-placeholder.png");
            }
        }
        private async void changeModeAnimationAsync()
        {
            switch (ViewModel.Mode)
            {
                case ProfileMode.Details:
                    {
                        await editConteiner.FadeTo(0, 350, Easing.SinInOut);
                        editConteiner.IsVisible = false;
                        detailsConteiner.IsVisible = true;
                        await detailsConteiner.FadeTo(1, 350, Easing.SinInOut);
                    }
                    break;
                case ProfileMode.Edit:
                    {
                        await detailsConteiner.FadeTo(0, 350, Easing.SinInOut);
                        detailsConteiner.IsVisible = false;
                        editConteiner.IsVisible = true;
                        await editConteiner.FadeTo(1, 350, Easing.SinInOut);
                    }
                    break;
            }

            await editConteiner.ScrollToAsync(0, 0, false);
            await detailsConteiner.ScrollToAsync(0, 0, false);
        }

        private async void EditProfilePictureButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    CompressionQuality = 30,
                    CustomPhotoSize = 50,
                });

                if (file == null)
                    return;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    ViewModel.ImageSource = ImageSource.FromStream(() => file.GetStream());

                    string ImageBytes = Convert.ToBase64String(memoryStream.ToArray());
                    ViewModel.Images = new List<ItemFormServices.Image> { new ItemFormServices.Image { BytesOfImage = ImageBytes } };
                    await ViewModel.UpdateUserAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}