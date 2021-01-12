using System.Collections.Generic;
using static Swap.Services.ItemFormServices;

namespace Swap.Models
{
    public class LoginUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUser : SignUpUser
    {
        public int Id { get; internal set; }
    }

    public class SignUpUserResult
    {
        public int Id { get; set; }
        public string Token { get; set; }
    }

    public class SignUpUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellPhone { get; set; }
        public string City { get; set; }
        public string FirebaseToken { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<Image> Images { get; set; }
    }

    public class LoginUserResult
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellPhone { get; set; }
        public string City { get; set; }
        public List<Image> Images { get; set; }
    }

    public class UserLocationUpdates
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}