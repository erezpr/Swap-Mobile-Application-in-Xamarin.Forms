using System;

[assembly: Xamarin.Forms.Dependency(typeof(Swap.Services.ILocationService))]
namespace Swap.Services
{
    public class GpsNotAvailableException : Exception
    {

    }
    public interface ILocationService
    {
        void OpenSettings();
    }
}