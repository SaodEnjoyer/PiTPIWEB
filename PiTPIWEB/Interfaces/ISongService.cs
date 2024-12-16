using Model;

namespace PiTPIWEB.Interfaces
{
    public interface ISongService
    {
        Task<IEnumerable<Song>> GetSongsByAuthorIdAsync(int authorId);
    }
}