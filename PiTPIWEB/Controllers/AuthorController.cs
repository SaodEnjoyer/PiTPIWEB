// Controllers/AuthorController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly MusicContext _context;

    public AuthorController(MusicContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuthor(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorById(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();
        return Ok(author);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAuthors()
    {
        var authors = await _context.Authors.ToListAsync();
        if (authors == null || !authors.Any())
            return NotFound("Авторы не найдены.");

        return Ok(authors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return NotFound();

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, Author updatedAuthor)
    {
        if (id != updatedAuthor.Id) return BadRequest();

        _context.Entry(updatedAuthor).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Authors.Any(a => a.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }
}
