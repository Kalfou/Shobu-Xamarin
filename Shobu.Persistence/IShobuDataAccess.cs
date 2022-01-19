using System.Threading.Tasks;

namespace Shobu.Persistence
{
    public interface IShobuDataAccess
    {

        Task<ShobuState> LoadAsync(string path);

        Task SaveAsync(string path, ShobuState state);
    }
}
