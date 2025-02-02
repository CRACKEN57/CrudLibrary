using CrudTodos.Business.Interfaces;
using CrudTodos.Data;
using CrudTodos.Data.Models;
using CrudTodos.Presentations.Components.Pages;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

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

            if (book != null)
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
        public byte[] GeneratePdfReport(List<Book> books)
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
            graphics.DrawString("Código", headerFont, XBrushes.White, new XPoint(50, 158));
            graphics.DrawString("Título", headerFont, XBrushes.White, new XPoint(200, 158));
            graphics.DrawString("Stock", headerFont, XBrushes.White, new XPoint(340, 158));
            graphics.DrawString("Autor", headerFont, XBrushes.White, new XPoint(467, 158));

            // Identificar el libro con el mayor stock
            var bookWithMaxStock = books.OrderByDescending(b => b.AmountBooks).FirstOrDefault();

            // Fila de datos
            int yPosition = 188;
            foreach (var book in books)
            {
                bool isMaxStock = book == bookWithMaxStock; // Validar si es el libro con mayor stock

                // Si es el libro con mayor stock, cambiar el fondo de la fila
                var rowBrush = isMaxStock ? XBrushes.Yellow : XBrushes.White;

                // Dibujar la fila
                graphics.DrawRectangle(new XSolidBrush(rowBrush), new XRect(20, yPosition - 16, 555, 20)); // Fondo de la fila
                graphics.DrawString(book.Id_Book, bodyFont, lineBrush, new XPoint(25, yPosition));
                graphics.DrawString(book.Title, bodyFont, lineBrush, new XPoint(125, yPosition));
                graphics.DrawString(book.AmountBooks.ToString(), bodyFont, lineBrush, new XPoint(335, yPosition));
                graphics.DrawString(book.Author.Name, bodyFont, lineBrush, new XPoint(415, yPosition));
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
