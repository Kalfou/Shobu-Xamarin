using Shobu.Model;
using System;
using System.Collections.ObjectModel;

namespace Shobu.ViewModel
{
    public class ShobuPersistanceViewModel : ViewModelBase
    {

        #region Fields

        private readonly ShobuPersistanceModel _model;

        #endregion

        #region Properties
        public DelegateCommand NewSaveCommand { get; private set; }

        public ObservableCollection<StoredShobuViewModel> StoredGames { get; private set; }

        #endregion

        #region Events

        public event EventHandler<ShobuPersistenceEventArgs> GameLoading;

        public event EventHandler<ShobuPersistenceEventArgs> GameSaving;

        #endregion

        #region Constructors

        public ShobuPersistanceViewModel(ShobuPersistanceModel model)
        {
            _model = model ?? throw new ArgumentNullException("model");

            _model.StoreChanged += new EventHandler(Model_StoreChanged);

            NewSaveCommand = new DelegateCommand(param => OnGameSaving((string)param));
            StoredGames = new ObservableCollection<StoredShobuViewModel>();
            UpdateStoredGames();
        }

        #endregion

        #region Private methods

        private void UpdateStoredGames()
        {
            StoredGames.Clear();

            foreach (StoredShobuModel item in _model.StoredGames)
            {
                StoredGames.Add(new StoredShobuViewModel
                {
                    Name = item.Name,
                    Modified = item.Modified,
                    LoadGameCommand = new DelegateCommand(param => OnGameLoading((string)param)),
                    SaveGameCommand = new DelegateCommand(param => OnGameSaving((string)param))
                });
            }
        }

        private void Model_StoreChanged(object sender, EventArgs e)
        {
            UpdateStoredGames();
        }

        private void OnGameLoading(string name)
        {
            GameLoading?.Invoke(this, new ShobuPersistenceEventArgs { Name = name });
        }

        private void OnGameSaving(string name)
        {
            GameSaving?.Invoke(this, new ShobuPersistenceEventArgs { Name = name });
        }

        #endregion

    }
}
