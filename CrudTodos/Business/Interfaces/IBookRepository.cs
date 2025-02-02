using CrudTodos.Data.Models;

namespace CrudTodos.Business.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<Book> GetBookByIdAsync(string id);
        Task AddBookAsync(Book book);
        Task DeleteBookAsync(string id);
        Task UpdateBookAsync(Book book);
        Task UpdateBookIfExits(Book book);
        Task<bool> IsBookUniqueAsync(string id);
        byte[] GeneratePdfReport(List<Book> books);
    }
}
