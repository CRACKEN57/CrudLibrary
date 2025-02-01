using CrudTodos.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel;

namespace CrudTodos.Business.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAuthorAsync();
        Task<Author> GetAuthorByIdAsync(String Author_Id);
        Task AddAuthorAsync(Author author);
        Task DeleteAuthorAsync(string Author_Id); 

        Task UpdateAuthorAsync(Author author);

        Task UpdateAuthorIfExitsAsync(Author author);

        Task<bool> IsAuthorUniqueAsync(string Author_Id);
    }
}
