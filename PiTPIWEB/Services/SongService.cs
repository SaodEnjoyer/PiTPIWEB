using Model;
using PiTPIWEB.Interfaces;

namespace PiTPIWEB.Services
{
    public class SongService : ISongService
    {
        private readonly MusicContext _context;

        public SongService(MusicContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<Song>> GetSongsByAuthorIdAsync(int authorId)
        {
            var songsByAuthor = _context.Songs.Where(s => s.AuthorId == authorId).ToList();
            return Task.FromResult<IEnumerable<Song>>(songsByAuthor);
        }

    }
}