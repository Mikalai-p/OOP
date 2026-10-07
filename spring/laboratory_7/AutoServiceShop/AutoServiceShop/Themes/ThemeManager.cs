using System;
using System.Windows;

namespace AutoServiceShop.Themes
{
    public static class ThemeManager
    {
        private static ResourceDictionary _currentTheme;
        public static event EventHandler ThemeChanged;

        public static void ChangeTheme(string themeName)
        {
            if (_currentTheme != null)
                Application.Current.Resources.MergedDictionaries.Remove(_currentTheme);

            string uri = $"Themes/{themeName}.xaml";
            _currentTheme = new ResourceDictionary { Source = new Uri(uri, UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Add(_currentTheme);

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}