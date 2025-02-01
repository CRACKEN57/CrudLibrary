using CrudTodos.Business.Interfaces;
using CrudTodos.Data;
using CrudTodos.Data.Models;
using CrudTodos.Presentations.Components.Pages;
using Microsoft.EntityFrameworkCore;

namespace CrudTodos.Business.Repositories
{
    public class BookRepository : IBookRepository
    {

        private readonly AppDbContext _Context;

        public BookRepository(AppDbContext context)
        {
            this._Context = context;
        }

        public async Task AddBookAsync(Book book)
        {
            await _Context.Books.AddAsync(book);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(string id)
        {
            var book = await _Context.Books.FindAsync(id);

            if(book != null)
            {
                _Context.Books.Remove(book);
                await _Context.SaveChangesAsync();
            }
        }

        public async Task<Book> GetBookByIdAsync(string id)
        {
            var book = await _Context.Books.Include(a => a.Author).FirstOrDefaultAsync(b => b.Id_Book == id);
            //para evitar este if nada mas se le pone un ? 
            if (book == null)
            {
                throw new KeyNotFoundException($"No se encontró el libro deseado con el ID{id}.");
            }
            return book;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _Context.Books.Include(a => a.Author).ToListAsync();
        }

        public async Task<bool> IsBookUniqueAsync(string id)
        {
            var existingBook = await _Context.Books.FirstOrDefaultAsync(b => b.Id_Book == id);
            return existingBook == null; // Retorna true si no existe, false si ya existe
        }

        public async Task UpdateBookAsync(Book book)
        {
            _Context.Books.Update(book);
            await _Context.SaveChangesAsync();
        }

        public async Task UpdateBookIfExits(Book book)
        {
            var ExistingBook = await _Context.Books
                .FirstOrDefaultAsync(a => a.Id_Book == book.Id_Book);

            if (ExistingBook == null)
                throw new Exception("El autor no existe en la base de datos.");

            //Asignar valores manualmente para que EF los detecte como modificados
            ExistingBook.Title = book.Title;
            ExistingBook.Price = book.Price;
            ExistingBook.AmountBooks = book.AmountBooks;
            ExistingBook.Edition_date = book.Edition_date;
            //Si Estado no se debe modificar, no lo tocamos

            await _Context.SaveChangesAsync();
            Console.WriteLine("Autor actualizado en la base de datos.");
        }
    }
}
