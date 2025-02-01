using CrudTodos.Business.Interfaces;
using CrudTodos.Data;
using CrudTodos.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudTodos.Business.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _Context;

        public AuthorRepository(AppDbContext context)
        {
            this._Context = context;
        }
        public async Task AddAuthorAsync(Author author)
        {
            author.Status = true;
            await _Context.Authors.AddAsync(author);
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteAuthorAsync(string Author_Id)
        {
            var AuthorToUpdate = await _Context.Authors.FindAsync(Author_Id);

            if (AuthorToUpdate != null)
            {
                AuthorToUpdate.Status = false; // Asegúrate de que la columna se llama así en la BD
                await _Context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Author>> GetAllAuthorAsync()
        {
                return await _Context.Authors
            .Where(a => a.Status == true) // Filtrar solo los autores activos
            .Include(b => b.Books)
            .ToListAsync();
        }

        public async Task<Author> GetAuthorByIdAsync(String Author_Id)
        {
            var authorFound= await _Context.Authors.Include(b=>b.Books).FirstOrDefaultAsync(a => a.Author_Id == Author_Id);
            //para evitar este if nada mas se le pone un ? Task<Author?>
            if (authorFound == null)
            {
                throw new KeyNotFoundException($"No se encontró el libro deseado con el ID{Author_Id}.");
            }
            return authorFound;
        }

        public async Task<bool> IsAuthorUniqueAsync(string Author_Id)
        {
            var existingAuthor = await _Context.Authors.FirstOrDefaultAsync(a=>a.Author_Id == Author_Id);
            return existingAuthor == null; // Retorna true si no existe, false si ya existe
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            _Context.Authors.Update(author);
            await _Context.SaveChangesAsync();
        }

        public async Task UpdateAuthorIfExitsAsync(Author author)
        {
            var existingAuthor = await _Context.Authors
                .FirstOrDefaultAsync(a => a.Author_Id == author.Author_Id);

            if (existingAuthor == null)
                throw new Exception("El autor no existe en la base de datos.");

            //Asignar valores manualmente para que EF los detecte como modificados
            existingAuthor.Name = author.Name;
            existingAuthor.Age = author.Age;
            existingAuthor.Biography = author.Biography;
            existingAuthor.Status = true;
            //Si Estado no se debe modificar, no lo tocamos

            await _Context.SaveChangesAsync();
            Console.WriteLine("Autor actualizado en la base de datos.");
        }

    }
}
