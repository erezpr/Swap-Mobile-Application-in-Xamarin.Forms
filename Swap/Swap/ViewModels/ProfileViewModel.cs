using Plugin.Media;
using Plugin.Media.Abstractions;
using Swap.Enums;
using Swap.Models;
using Swap.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Swap.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public event Action ModeChanged;
        public List<ItemFormServices.Image> Images { get; set; }

        private ProfileMode m_Mode;
        public ProfileMode Mode
        {
            get { return m_Mode; }
            set
            {
                SetValue(ref m_Mode, value);
                ModeChanged?.Invoke();
            }
        }

        private bool m_IsFacebookUser;
        public bool IsFacebookUser
        {
            get { return m_IsFacebookUser; }
            set { SetValue(ref m_IsFacebookUser, value); }
        }

        private string m_Massage;
        public string Message
        {
            get { return m_Massage; }
            set { SetValue(ref m_Massage, value); }
        }

        private string m_UserName;
        public string UserName
        {
            get { return m_UserName; }
            set { SetValue(ref m_UserName, value); }
        }

        private string m_Email;
        public string Email
        {
            get { return m_Email; }
            set { SetValue(ref m_Email, value); }
        }

        private string m_Password;
        public string Password
        {
            get { return m_Password; }
            set { SetValue(ref m_Password, value); }
        }

        private string m_NewPassword;
        public string NewPassword
        {
            get { return m_NewPassword; }
            set { SetValue(ref m_NewPassword, value); }
        }

        private string m_ConfirmNewPassword;
        public string ConfirmNewPassword
        {
            get { return m_ConfirmNewPassword; }
            set { SetValue(ref m_ConfirmNewPassword, value); }
        }

        private string m_PhoneNumber;
        public string PhoneNumber
        {
            get { return m_PhoneNumber; }
            set { SetValue(ref m_PhoneNumber, value); }
        }

        private string m_Address;
        public string Address
        {
            get { return m_Address; }
            set { SetValue(ref m_Address, value); }
        }

        private string m_City;
        public string City
        {
            get { return m_City; }
            set { SetValue(ref m_City, value); }
        }

        private string m_Neighborhood;
        public string Neighborhood
        {
            get { return m_Neighborhood; }
            set { SetValue(ref m_Neighborhood, value); }
        }

        private bool m_IsUserLogin;
        public bool IsUserLogin
        {
            get { return m_IsUserLogin; }
            set { SetValue(ref m_IsUserLogin, value); }
        }

        private ImageSource m_ImageSource;
        public ImageSource ImageSource
        {
            get { return m_ImageSource; }
            set { SetValue(ref m_ImageSource, value); }
        }

        public ProfileViewModel()
        {
            Mode = ProfileMode.Details;
            if (string.IsNullOrWhiteSpace(app.ProfileImageBytes) == false)
            {
                byte[] byteArray = Convert.FromBase64String(app.ProfileImageBytes);
                ImageSource = ImageSource.FromStream(() =>
                    new MemoryStream(byteArray)
                );
            }
            if (ImageSource == null)
            {
                ImageSource = ImageSource.FromResource("Swap.Images.profile-placeholder.png");
            }

            IsUserLogin = !string.IsNullOrWhiteSpace(app.Token);
            UpdateInfo();
        }

        private ICommand m_PushRegiterPage;
        public ICommand PushRegiterPage => m_PushRegiterPage ?? (m_PushRegiterPage = new Command(async () =>
        {
            await Shell.Current.GoToAsync("//register");
        }));

        private ICommand m_EditProfilePictureButton;
        public ICommand EditProfilePictureButton => m_EditProfilePictureButton ?? (m_EditProfilePictureButton = new Command(async () =>
        {

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Shell.Current.DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
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
                ImageSource = ImageSource.FromStream(() => file.GetStream());

                string ImageBytes = Convert.ToBase64String(memoryStream.ToArray());
                Images = new List<ItemFormServices.Image> { new ItemFormServices.Image { BytesOfImage = ImageBytes } };
                await UpdateUserAsync();
            }
        }));

        private ICommand m_SwitchMode;
        public ICommand SwitchMode => m_SwitchMode ?? (m_SwitchMode = new Command(() =>
        {
            Mode = Mode == ProfileMode.Details ? ProfileMode.Edit : ProfileMode.Details;
        }));

        private ICommand m_Cancel;
        public ICommand Cancel => m_Cancel ?? (m_Cancel = new Command(() =>
        {
            UpdateInfo();
            Mode = Mode == ProfileMode.Details ? ProfileMode.Edit : ProfileMode.Details;
        }));

        public void UpdateInfo()
        {
            Email = app.Email;
            UserName = app.UserName;
            City = app.City;
            Neighborhood = app.Neighborhood;
            PhoneNumber = app.PhoneNumber;
            Address = string.Format("{0}{1}{2}", app.City, string.IsNullOrWhiteSpace(app.Neighborhood) ? "" : ", ", app.Neighborhood);

            if (string.IsNullOrWhiteSpace(app.ProfileImageBytes) == false)
            {
                byte[] byteArray = Convert.FromBase64String(app.ProfileImageBytes);
                ImageSource = ImageSource.FromStream(() =>
                    new MemoryStream(byteArray)
                );
            }
        }

        private ICommand m_UpdateUserProfile;
        public ICommand UpdateUserProfile => m_UpdateUserProfile ?? (m_UpdateUserProfile = new Command(async () =>
        {
            if (checkInputValidity())
            {
                Message = "";
                try
                {
                    await ServerFacade.Users.UpdateUserAsync(new UpdateUser
                    {
                        Id = app.UserId,
                        FirstName = UserName,
                        CellPhone = PhoneNumber,
                        City = City,
                        LastName = Neighborhood
                    });

                    Mode = ProfileMode.Details;
                    app.Email = Email;
                    app.UserName = UserName;
                    app.PhoneNumber = PhoneNumber;
                    app.City = City;
                    app.Neighborhood = Neighborhood;
                    Address = string.Format("{0}{1}{2}", City, string.IsNullOrWhiteSpace(Neighborhood) ? "" : ", ", Neighborhood);
                }
                catch (Exception)
                {
                    Email = app.Email;
                    UserName = app.UserName;
                    PhoneNumber = app.PhoneNumber;
                    Address = string.Format("{0}{1}{2}", app.City, string.IsNullOrWhiteSpace(app.Neighborhood) ? "" : ", ", app.Neighborhood);
                    await Shell.Current.DisplayAlert("שגיאה", "משהו השתבש נסה שנית", "אישור");
                }

            }
        }));

        public async Task UpdateUserAsync()
        {
            try
            {
                await ServerFacade.Users.UpdateUserAsync(new UpdateUser
                {
                    Id = app.UserId,
                    Images = Images
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאה", "משהו השתבש נסה שנית", "אישור");
            }
        }

        private bool checkInputValidity()
        {
            if (Mode.Equals(ProfileMode.Edit) && !string.IsNullOrWhiteSpace(PhoneNumber))
            {
                if (StringValidationService.IsValid(PhoneNumber, ValidationType.PhoneNumber) == false)
                {
                    Message = "מספר טלפון לא תקין";
                    return false;
                }
            }
            return true;
        }
    }
}