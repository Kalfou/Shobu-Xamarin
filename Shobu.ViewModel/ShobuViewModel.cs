using Shobu.Model;
using System;
using System.Collections.ObjectModel;

namespace Shobu.ViewModel
{
    public class ShobuViewModel : ViewModelBase
    {
        #region Fields

        private readonly ShobuModel _model;

        #endregion

        #region Properties

        public ObservableCollection<ShobuField> BlackFields { get; set; }

        public ObservableCollection<ShobuField> WhiteFields { get; set; }

        public DelegateCommand NewGameCommand { get; private set; }

        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand LoadGameCommand { get; private set; }

        public DelegateCommand OpenMenuCommand { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

        #endregion

        #region Events

        public event EventHandler GameOver;

        public event EventHandler LoadGame;

        public event EventHandler SaveGame;

        public event EventHandler OpenMenu;

        #endregion

        #region Constructors

        public ShobuViewModel(ShobuModel model)
        {
            _model = model;

            _model.GameChanged += new EventHandler<ShobuEventArgs>(Model_GameChanged);
            _model.StepGame += new EventHandler<ShobuEventArgs>(Model_StepGame);

            NewGameCommand = new DelegateCommand(param => _model.NewGame());
            LoadGameCommand = new DelegateCommand(param => LoadGame?.Invoke(this, EventArgs.Empty));
            SaveGameCommand = new DelegateCommand(param => SaveGame?.Invoke(this, EventArgs.Empty));
            OpenMenuCommand = new DelegateCommand(param => OpenMenu?.Invoke(this, EventArgs.Empty));
            ResetCommand = new DelegateCommand(param => ResetStep());

            BlackFields = new ObservableCollection<ShobuField>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    BlackFields.Add(new ShobuField
                    {
                        X = i,
                        Y = j,
                        Side = _model[i,j],
                        Number = (i * 8) + j,
                        Color = _model.FieldState(i, j),
                        FieldClickCommand = new DelegateCommand(param => FieldClick(Convert.ToInt32(param)))
                    });
                }
            }

            WhiteFields = new ObservableCollection<ShobuField>();
            for (int i = 4; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    WhiteFields.Add(new ShobuField
                    {
                        X = i,
                        Y = j,
                        Side = _model[i, j],
                        Number = (i * 8) + j,
                        Color = _model.FieldState(i, j),
                        FieldClickCommand = new DelegateCommand(param => FieldClick(Convert.ToInt32(param)))
                    });
                }
            }
        }

        #endregion

        #region Event handlers

        private void Model_GameChanged(object sender, ShobuEventArgs e)
        {
            RefreshColors();
            RefreshBoard();
        }

        private void Model_StepGame(object sender, ShobuEventArgs e)
        {
            RefreshColors();
            RefreshBoard();

            if (e.Victor != 0)
            {
                GameOver?.Invoke(this, EventArgs.Empty);
            }
        }

        private void FieldClick(int index)
        {
            if (index > 31)
            {
                _model.FieldClick(WhiteFields[index - 32].X, WhiteFields[index - 32].Y);
            }
            else
            {
                _model.FieldClick(BlackFields[index].X, BlackFields[index].Y);
            }
        }

        #endregion

        #region Private methods

        private void ResetStep()
        {
            _model.ResetStep();
            RefreshColors();
        }

        private void RefreshBoard()
        {
            foreach (ShobuField field in BlackFields)
            {
                field.Side = _model[field.X, field.Y];
                field.Color = _model.FieldState(field.X, field.Y);
            }

            foreach (ShobuField field in WhiteFields)
            {
                field.Side = _model[field.X, field.Y];
                field.Color = _model.FieldState(field.X, field.Y);
            }
        }
        
        private void RefreshColors()
        {
            foreach (ShobuField field in BlackFields)
            {
                field.Color = _model.FieldState(field.X, field.Y);
            }

            foreach (ShobuField field in WhiteFields)
            {
                field.Color = _model.FieldState(field.X, field.Y);
            }
        }

        #endregion
    }
}
