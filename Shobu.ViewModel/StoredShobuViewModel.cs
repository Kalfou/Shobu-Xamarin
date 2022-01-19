using System;

namespace Shobu.ViewModel
{
    public class StoredShobuViewModel : ViewModelBase
    {

        #region Fields

        private string _name;
        private DateTime _modified;

        #endregion

        #region Properties
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Modified
        {
            get { return _modified; }
            set
            {
                if (_modified != value)
                {
                    _modified = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand LoadGameCommand { get; set; }

        public DelegateCommand SaveGameCommand { get; set; }

        #endregion

    }
}
