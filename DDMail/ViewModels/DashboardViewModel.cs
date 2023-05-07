using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System;
using System.Net;
using System.Threading.Tasks;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using System.Net.Http;
using DDMail.Services;
using Wpf.Ui.Common;
using Clipboard = System.Windows.Clipboard;

namespace DDMail.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        private readonly ISnackbarService _snackbarService;
        private readonly AddressService _addressService;

        const string AddressDomain = "@duck.com";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Address))]
        [NotifyCanExecuteChangedFor(nameof(CopyCommand))]
        private string? _alias;

        public string? Address => string.IsNullOrEmpty(_alias) ? null : $"{_alias}{AddressDomain}";

        public DashboardViewModel(ISnackbarService snackbarService, AddressService addressService)
        {
            _snackbarService = snackbarService;
            _addressService = addressService;
            Alias = _addressService.GetNextAlias();
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        [RelayCommand]
        private async Task OnGenerate()
        {
            try
            {
                Alias = await _addressService.GenerateNewAlias();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _snackbarService.Show("Error", "Verify your token", SymbolRegular.LockClosed24,
                    ControlAppearance.Danger);
            }
            catch
            {
                _snackbarService.Show("Error", "Unknown error", SymbolRegular.ErrorCircle24,
                    ControlAppearance.Danger);
            }
        }
        
        private bool CanCopy() => Address is not null;

        [RelayCommand(CanExecute = nameof(CanCopy))]
        private void OnCopy()
        {
            try
            {
                if (Address == null) return;

                Clipboard.Clear();
                Clipboard.SetText(Address);
                _snackbarService.Show("Address copied!", Address, SymbolRegular.Copy24, ControlAppearance.Success);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
