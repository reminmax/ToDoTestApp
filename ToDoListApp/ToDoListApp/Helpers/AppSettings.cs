using System;
using Xamarin.Essentials;

namespace ToDoListApp.Helpers
{
    public static class AppSettings
    {
        public static string UserName
        {
            get
            {
                if (Preferences.ContainsKey("UserName"))
                    return Preferences.Get("UserName", String.Empty);

                return string.Empty;
            }
            set => Preferences.Set("UserName", value);
        }

        public static string AuthToken
        {
            get
            {
                if (Preferences.ContainsKey("AuthToken"))
                    return Preferences.Get("AuthToken", String.Empty);

                return string.Empty;
            }
            set
            {
                Preferences.Set("AuthToken", value);

                if (!string.IsNullOrEmpty(value))
                    AuthTokenCreationDate = DateTime.Now;
                else
                    AuthTokenCreationDate = DateTime.MinValue;
            }
        }

        public static DateTime AuthTokenCreationDate
        {
            get
            {
                if (Preferences.ContainsKey("AuthTokenCreationDate"))
                    return Preferences.Get("AuthTokenCreationDate", DateTime.MinValue);
                else
                    return new DateTime();
            }
            set => Preferences.Set("AuthTokenCreationDate", value);
        }

        public static bool IsUserLoggedIn() =>
            !string.IsNullOrEmpty(AuthToken) &
            AuthTokenCreationDate.AddHours(ConstantValues.TokenExpirationTimeHours) >= DateTime.Now;
    }
}
