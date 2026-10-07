using System.Windows.Input;
using AutoServiceShop.Commands;
using AutoServiceShop.Models;
using AutoServiceShop.Resources;
using AutoServiceShop.Themes;
using AutoServiceShop.Properties;

namespace AutoServiceShop.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private UserProfile _profile;

        public UserProfile Profile
        {
            get => _profile;
            set { _profile = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ProfileViewModel()
        {
            // Загружаем сохранённые данные из настроек
            Profile = new UserProfile
            {
                Name = Settings.Default.UserName,
                Email = Settings.Default.UserEmail,
                Language = Settings.Default.Language,
                Theme = Settings.Default.Theme
            };

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save(object param)
        {
            // Сохраняем в настройки
            Settings.Default.UserName = Profile.Name;
            Settings.Default.UserEmail = Profile.Email;
            Settings.Default.Language = Profile.Language;
            Settings.Default.Theme = Profile.Theme;
            Settings.Default.Save();

            // Применяем язык и тему
            LocalizationManager.ChangeLanguage(Profile.Language);
            ThemeManager.ChangeTheme(Profile.Theme);

            // Закрываем окно
            if (param is System.Windows.Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void Cancel(object param)
        {
            if (param is System.Windows.Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}