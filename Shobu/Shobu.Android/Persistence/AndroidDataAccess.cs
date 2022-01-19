using Shobu.Droid.Persistence;
using Shobu.Model;
using Shobu.Persistence;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidDataAccess))]
namespace Shobu.Droid.Persistence
{
    public class AndroidDataAccess : IShobuDataAccess
    {
        public async Task<ShobuState> LoadAsync(String path)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            string[] values = (await Task.Run(() => File.ReadAllText(filePath))).Split(' ');

            ShobuState state = new ShobuState
            {
                Turn = int.Parse(values[0])
            };

            int valueIndex = 1;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        state[i, j, k] = int.Parse(values[valueIndex]);
                        valueIndex++;

                    }
                }
            }

            return state;
        }

        public async Task SaveAsync(string path, ShobuState state)
        {
            string text = state.Turn.ToString();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        text += " " + state[i, j, k];
                    }
                }
            }

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);

            await Task.Run(() => File.WriteAllText(filePath, text));
        }
    }
}