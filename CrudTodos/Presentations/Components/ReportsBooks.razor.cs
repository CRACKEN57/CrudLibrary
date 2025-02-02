using CrudTodos.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace CrudTodos.Presentations.Components
{
    public partial class ReportsBooks
    {
        private List<Book> filteredBooks = new();
        private List<Author> Authors = new();
        private IEnumerable<Book> books = new List<Book>();
        private List<Book> BookToRep = new();

        [Inject]
        private IJSRuntime Js { get; set; }

        private string searchTerm { get; set; } = string.Empty;
        private Book selectedBook = new();

        // Paginación
        private int currentPage = 1;
        private int pageSize = 5;
        private int totalPages => (int)Math.Ceiling((double)filteredBooks.Count / pageSize);

        // Ordenación
        private bool sortAscending = true;
        private string sortColumn = "Title";

        protected override async Task OnInitializedAsync()
        {
            books = await BookService.GetBooksAsync();
            BookToRep = (await BookService.GetBooksAsync()).ToList();
            filteredBooks = books.ToList();
            Authors = (await AuthorService.GetAllAuthorAsync()).ToList();
        }

        private List<Book> PaginateBooks()
        {
            return filteredBooks
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private async Task SortBy(string column)
        {
            if (sortColumn == column)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortColumn = column;
                sortAscending = true;
            }

            // Ordenar directamente filteredBooks en memoria
            filteredBooks = (sortAscending
                ? books.OrderBy(b => GetPropertyValue(b, sortColumn))
                : books.OrderByDescending(b => GetPropertyValue(b, sortColumn))
            ).ToList();
        }

        private object GetPropertyValue(Book book, string propertyName)
        {
            PropertyInfo prop = typeof(Book).GetProperty(propertyName);
            return prop != null ? prop.GetValue(book, null) ?? "" : "";
        }
        private async Task DownloadPdf()
        {
            var pdfContent = BookService.GeneratePdfReport(BookToRep);
            // Usar JSInterop para permitir que el cliente descargue el archivo PDF
            var fileName = "reporte_libros.pdf";
            var fileContent = new ByteArrayContent(pdfContent);
            // Convertir el contenido del ByteArrayContent a un array de bytes
            var contentBytes = await fileContent.ReadAsByteArrayAsync();
            // Llamar al JS para descargar el archivo
            await Js.InvokeVoidAsync("blazorDownloadFile", fileName,
           contentBytes);

        }
    }
}
