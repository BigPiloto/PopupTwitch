using System.Globalization;
using System.Resources;

namespace PopupTwitch.Resources
{
    public static class Strings
    {
        private static readonly ResourceManager rm =
            new ResourceManager("PopupTwitch.Resources.Strings", typeof(Strings).Assembly);

        public static string Get(string key)
        {
            return rm.GetString(key, CultureInfo.CurrentUICulture)
                ?? $"[{key}]";
        }

        // atalhos convenientes
        public static string Default_PopupMessage => Get(nameof(Default_PopupMessage));
        public static string Error_PopupTooShort => Get(nameof(Error_PopupTooShort));
        public static string Error_IdleTooShort => Get(nameof(Error_IdleTooShort));
    }
}
