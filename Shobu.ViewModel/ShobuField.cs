namespace Shobu.ViewModel
{
    public class ShobuField : ViewModelBase
    {
        #region Fields

        private int _color;
        private int _side;
        private string _text;

        #endregion

        #region Properties

        public int X { get; set; }

        public int Y { get; set; }

        public int Number { get; set; }

        public int Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Side
        {
            get => _side;
            set
            {
                if (_side != value)
                {
                    _side = value;
                    Text = value == 0 ? "" : "⬤";
                    OnPropertyChanged();
                }
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand FieldClickCommand { get; set; }

        #endregion
    }
}
