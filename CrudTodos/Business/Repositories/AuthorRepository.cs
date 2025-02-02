using CrudTodos.Business.Interfaces;
using CrudTodos.Data;
using CrudTodos.Data.Models;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

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

        public byte[] GeneratePdfReport(List<Author> authors)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var graphics = XGraphics.FromPdfPage(page);

            // Fuentes y colores
            var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
            var headerFont = new XFont("Arial", 12, XFontStyle.Bold);
            var bodyFont = new XFont("Arial", 12, XFontStyle.Regular);
            var reportTitleFont = new XFont("Arial", 14, XFontStyle.Bold);
            var headerColor = XColor.FromArgb(8, 163, 252);  // Azul oscuro
            var lineColor = XColor.FromArgb(0x1D, 0x21, 0x21);   // Gris oscuro
            var headerBrush = new XSolidBrush(headerColor);
            var lineBrush = new XSolidBrush(lineColor);

            // Título del reporte
            graphics.DrawString("Reporte de Biblioteca", titleFont, XBrushes.Crimson, new XPoint(210, 50));
            graphics.DrawString("Reporte del Autor con Más Libros", reportTitleFont, XBrushes.Black, new XPoint(180, 100));

            // Cabecera de la tabla
            graphics.DrawRectangle(headerBrush, new XRect(20, 145, 100, 20)); // Código
            graphics.DrawRectangle(headerBrush, new XRect(120, 145, 200, 20)); // Título
            graphics.DrawRectangle(headerBrush, new XRect(320, 145, 80, 20));  // Stock
            graphics.DrawRectangle(headerBrush, new XRect(400, 145, 175, 20)); // Autor
            graphics.DrawString("Id", headerFont, XBrushes.White, new XPoint(50, 158));
            graphics.DrawString("Nombre", headerFont, XBrushes.White, new XPoint(115, 158));
            graphics.DrawString("Edad", headerFont, XBrushes.White, new XPoint(255, 158));
            graphics.DrawString("Libros Publicados", headerFont, XBrushes.White, new XPoint(310, 158));
            graphics.DrawString("Biografia", headerFont, XBrushes.White, new XPoint(467, 158));

            // Identificar el Author con el mayor numero de libros
            var bookWithMaxStock = authors.OrderByDescending(b => b.Books.Count).FirstOrDefault();

            // Fila de datos
            int yPosition = 188;
            foreach (var author in authors)
            {
                bool isMaxStock = author == bookWithMaxStock; // Validar si es el libro con mayor stock

                // Si es el libro con mayor stock, cambiar el fondo de la fila
                var rowBrush = isMaxStock ? XBrushes.Yellow : XBrushes.White;

                // Dibujar la fila
                graphics.DrawRectangle(new XSolidBrush(rowBrush), new XRect(20, yPosition - 16, 555, 20)); // Fondo de la fila
                graphics.DrawString(author.Author_Id, bodyFont, lineBrush, new XPoint(25, yPosition));
                graphics.DrawString(author.Name, bodyFont, lineBrush, new XPoint(115, yPosition));
                graphics.DrawString(author.Age.ToString(), bodyFont, lineBrush, new XPoint(260, yPosition));
                graphics.DrawString(author.Books.Count.ToString(), bodyFont, lineBrush, new XPoint(350, yPosition));
                graphics.DrawString(author.Biography, bodyFont, lineBrush, new XPoint(467, yPosition));
                graphics.DrawLine(XPens.Gray, 21, yPosition + 4, 574, yPosition + 4); // Línea divisoria

                yPosition += 21; // Desplazamiento para la siguiente fila
            }

            // Guardar el documento PDF en un MemoryStream y devolverlo como byte[]
            using (var stream = new MemoryStream())
            {
                document.Save(stream);
                return stream.ToArray();
            }
        }


    }
}
