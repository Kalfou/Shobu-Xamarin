using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shobu.Persistence
{
    public interface IStore
    {
        Task<IEnumerable<string>> GetFiles();

        Task<DateTime> GetModifiedTime(string name);
    }
}
