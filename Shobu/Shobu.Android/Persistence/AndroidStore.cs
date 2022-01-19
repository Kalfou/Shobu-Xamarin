using Shobu.Droid.Persistence;
using Shobu.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidStore))]
namespace Shobu.Droid.Persistence
{
    public class AndroidStore : IStore
    {
        public async Task<IEnumerable<string>> GetFiles()
        {
            return await Task.Run(() => Directory.GetFiles(Environment
                                        .GetFolderPath(Environment.SpecialFolder.Personal))
                                        .Select(file => Path.GetFileName(file)));
        }

        public async Task<DateTime> GetModifiedTime(string name)
        {
            FileInfo info = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), name));

            return await Task.Run(() => info.LastWriteTime);
        }
    }
}