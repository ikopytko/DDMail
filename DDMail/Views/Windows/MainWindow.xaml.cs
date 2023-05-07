using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace DDMail.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        public ViewModels.MainWindowViewModel ViewModel
        {
            get;
        }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService,
            ISnackbarService snackbarService, INavigationService navigationService)
        {
            this.Loaded += new RoutedEventHandler(Window_Loaded);
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            SetPageService(pageService);

            snackbarService.SetSnackbarControl(RootSnackbar);
            navigationService.SetNavigationControl(RootNavigation);

            navigationService.GetNavigationControl().Navigated += (sender, args) =>
            {
                ViewModel.ApplicationTitle = (string) args.CurrentPage.Content;
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        #region INavigationWindow methods

        public Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        private void TitleBar_OnCloseClicked(object sender, RoutedEventArgs e)
        {
            if (Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down)
            {
                Application.Current.Shutdown();
                return;
            }
            e.Handled= true;
            WindowState = WindowState.Minimized;
            Hide();
        }

        private void TrayMenuItem_OnShow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            Activate();
            ShowWindow();
            Focus();
        }

        private void TrayMenuItem_OnExit(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
    }
}