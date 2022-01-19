using Shobu.Model;
using Shobu.Persistence;
using Shobu.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Shobu
{
    public partial class App : Application
    {

        #region Fields

        private readonly IStore _store;
        private readonly IShobuDataAccess _access;

        private readonly ShobuModel _model;
        private readonly ShobuPersistanceModel _persistanceModel;

        private readonly ShobuViewModel _viewModel;
        private readonly ShobuPersistanceViewModel _persistanceViewModel;

        private readonly NavigationPage _mainPage;
        private readonly ShobuPage _shobuPage;
        private readonly MenuPage _menuPage;
        private readonly SaveGamePage _savePage;
        private readonly LoadGamePage _loadPage;

        #endregion

        #region Constructor

        public App()
        {
            InitializeComponent();

            _access = DependencyService.Get<IShobuDataAccess>();
            
            _model = new ShobuModel(_access);

            _viewModel = new ShobuViewModel(_model);

            _viewModel.GameOver += ViewModel_GameOver;
            _viewModel.OpenMenu += ViewModel_OpenMenu;
            _viewModel.SaveGame += ViewModel_SaveGame;
            _viewModel.LoadGame += ViewModel_LoadGame;

            _store = DependencyService.Get<IStore>();
            _persistanceModel = new ShobuPersistanceModel(_store);
            _persistanceViewModel = new ShobuPersistanceViewModel(_persistanceModel);

            _persistanceViewModel.GameSaving += PersistanceViewModel_GameSaving;
            _persistanceViewModel.GameLoading += PersistanceViewModel_GameLoading;

            _shobuPage = new ShobuPage
            {
                BindingContext = _viewModel
            };

            _menuPage = new MenuPage
            {
                BindingContext = _viewModel
            };

            _loadPage = new LoadGamePage
            {
                BindingContext = _persistanceViewModel
            };

            _savePage = new SaveGamePage
            {
                BindingContext = _persistanceViewModel
            };

            _mainPage = new NavigationPage(_shobuPage);
            _mainPage.BarBackgroundColor = Color.FromHex("#A0522D");

            MainPage = _mainPage;
        }

        #endregion

        #region Event handlers
        private async void PersistanceViewModel_GameLoading(object sender, ShobuPersistenceEventArgs e)
        {
            await _mainPage.PopAsync();

            try
            {
                await _model.LoadGameAsync(e.Name);

                await _mainPage.PopAsync();
                await MainPage.DisplayAlert("Shobu", "Thy game approches.", "Wondorous!");
            }
            catch
            {
                await MainPage.DisplayAlert("Shobu", "Thy game has eluded you.", "Begone!");
            }
        }

        private async void PersistanceViewModel_GameSaving(object sender, ShobuPersistenceEventArgs e)
        {
            await _mainPage.PopAsync();

            try
            {
                await _model.SaveGameAsync(e.Name);
                await MainPage.DisplayAlert("Shobu", "The game has been captured.", "Wondorous!");
            }
            catch
            {
                await MainPage.DisplayAlert("Shobu", "The game has eluded us.", "Begone!");
            }
        }

        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            await _persistanceModel.UpdateAsync();
            await _mainPage.PushAsync(_savePage);
        }

        private async void ViewModel_LoadGame(object sender, EventArgs e)
        {
            await _persistanceModel.UpdateAsync();
            await _mainPage.PushAsync(_loadPage);
        }

        private async void ViewModel_OpenMenu(object sender, EventArgs e)
        {
            await _mainPage.PushAsync(_menuPage);
        }
        private async void ViewModel_GameOver(object sender, EventArgs e)
        {
            await MainPage.DisplayAlert("Rejoice!", "Thy enemies are vanquished!", "Glorious!");

            _model.NewGame();
        }

        #endregion

        #region Application methods

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            try
            {
                Task.Run(async () => await _model.SaveGameAsync("SuspendedGame"));
            }
            catch{}
        }

        protected override void OnResume()
        {
            try
            {
                Task.Run(async () =>
                {
                    await _model.LoadGameAsync("SuspendedGame");
                });
            }
            catch { }
        }

        #endregion

    }
}
