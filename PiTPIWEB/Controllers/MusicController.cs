// Controllers/MusicController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    private readonly MusicContext _context;

    public MusicController(MusicContext context)
    {
        _context = context;
    }

    [HttpGet("songs")]
    public async Task<IActionResult> GetAllSongs()
    {
        var songs = await _context.Songs.ToListAsync();
        if (songs == null || !songs.Any())
            return NotFound("Песни не найдены.");

        return Ok(songs);
    }

    [HttpGet("authors/{authorId}/songs")]
    public async Task<IActionResult> GetSongsByAuthor(int authorId)
    {
        var songs = await _context.Songs
                                   .Where(s => s.AuthorId == authorId)
                                   .ToListAsync();

        if (songs == null || !songs.Any())
            return NotFound($"Песни для автора с ID {authorId} не найдены.");

        return Ok(songs);
    }
}
