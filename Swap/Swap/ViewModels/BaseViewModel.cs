using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Swap.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private bool m_IsBusy;
        public bool IsBusy
        {
            get { return m_IsBusy; }
            set { SetValue(ref m_IsBusy, value); }
        }

        public readonly App app = Application.Current as App;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return;
            }

            backingField = value;

            OnPropertyChanged(propertyName);
        }
    }
}
