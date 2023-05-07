using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using DDMail.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace DDMail.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private readonly ISnackbarService _snackbarService;
        private readonly TokenService _tokenService;

        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ThemeType _currentTheme = ThemeType.Unknown;

        [ObservableProperty]
        private ObservableCollection<ThemeType> _themes;

        [ObservableProperty]
        private string? _token = String.Empty;

        public SettingsViewModel(ISnackbarService snackbarService, TokenService tokenService)
        {
            _snackbarService = snackbarService;
            _tokenService = tokenService;
            _token = _tokenService.GetToken();
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            CurrentTheme = Theme.GetAppTheme();
            AppVersion = $"DDMail - {GetAssemblyVersion()}";

            Themes = new ObservableCollection<ThemeType>(new[]
            {
                ThemeType.Dark,
                ThemeType.Light,
            });

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? String.Empty;
        }

        partial void OnCurrentThemeChanged(ThemeType value)
        {
            Theme.Apply(value);
        }

        partial void OnTokenChanged(string? value)
        {
            _tokenService.SaveToken(value);
            _snackbarService.Show("Token saved");
        }
    }
}
