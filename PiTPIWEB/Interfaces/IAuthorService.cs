using Model;

namespace PiTPIWEB.Interfaces
{
    public interface IAuthorService
    {
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author> CreateAuthorAsync(Author author);
        Task<bool> UpdateAuthorAsync(Author updatedAuthor);
        Task<bool> DeleteAuthorAsync(int id);
    }
}
