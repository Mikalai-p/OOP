using System;
using System.Windows;

namespace AutoServiceShop.Resources
{
    public static class LocalizationManager
    {
        private static ResourceDictionary _currentDictionary;
        public static event EventHandler LanguageChanged;

        public static void ChangeLanguage(string culture)
        {
            if (_currentDictionary != null)
                Application.Current.Resources.MergedDictionaries.Remove(_currentDictionary);

            string uri = $"Resources/Strings.{culture}.xaml";
            _currentDictionary = new ResourceDictionary { Source = new Uri(uri, UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Add(_currentDictionary);

            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}