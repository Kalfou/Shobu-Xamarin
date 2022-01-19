using Shobu.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shobu.Model
{
    public class ShobuPersistanceModel
    {
        #region Fields

        private readonly IStore _store;

        #endregion

        #region Properties

        public List<StoredShobuModel> StoredGames { get; private set; }

        #endregion

        #region Events

        public event EventHandler StoreChanged;

        #endregion

        #region Constructors

        public ShobuPersistanceModel(IStore store)
        {
            _store = store;

            StoredGames = new List<StoredShobuModel>();
        }

        #endregion

        #region public Methods

        public async Task UpdateAsync()
        {
            if (_store == null)
            {
                return;
            }

            StoredGames.Clear();

            
            foreach (string name in await _store.GetFiles())
            {
                if (name == "SuspendedGame")
                {
                    continue;
                }

                StoredGames.Add(new StoredShobuModel
                {
                    Name = name,
                    Modified = await _store.GetModifiedTime(name)
                });
            }

            StoredGames = StoredGames.OrderByDescending(item => item.Modified).ToList();

            StoreChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
