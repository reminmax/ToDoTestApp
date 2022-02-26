using System;
using Xamarin.Essentials;

namespace ToDoListApp.Helpers
{
    public static class AppSettings
    {
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
                AuthTokenCreationDate = DateTime.Now;
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
            AuthToken != string.Empty &&
            AuthTokenCreationDate.AddHours(ConstantValues.TokenExpirationTimeHours) <= DateTime.Now;
    }
}
