using CrudTodos.Data.Models;
using CrudTodos.Presentations.Components.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace CrudTodos.Presentations.Components
{
    public partial class ReportsAuthors
    {
        private IEnumerable<Author> authors = new List<Author>();
        private List<Author> authorsToRep = new();

        [Inject]
        private IJSRuntime Js { get; set; }
        protected override async Task OnInitializedAsync()
        {
            // Fetch authors from a service or database
            authors = await AuthorService.GetAllAuthorAsync() ?? new List<Author>();
            authorsToRep = (await AuthorService.GetAllAuthorAsync()).ToList();
        }

        // Ordenación
        private bool sortAscending = true;
        private string sortColumn = "Title";

        private object GetPropertyValue(Author author, string propertyName)
        {
            PropertyInfo prop = typeof(Author).GetProperty(propertyName);
            return prop != null ? prop.GetValue(author, null) ?? "" : "";
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
            authors = (sortAscending
                ? authors.OrderBy(b => GetPropertyValue(b, sortColumn))
                : authors.OrderByDescending(b => GetPropertyValue(b, sortColumn))
            ).ToList();
        }
        private async Task DownloadPdf()
        {
            var pdfContent = AuthorService.GeneratePdfReport(authorsToRep);
            // Usar JSInterop para permitir que el cliente descargue el archivo PDF
            var fileName = "reporte_Autores.pdf";
            var fileContent = new ByteArrayContent(pdfContent);
            // Convertir el contenido del ByteArrayContent a un array de bytes
            var contentBytes = await fileContent.ReadAsByteArrayAsync();
            // Llamar al JS para descargar el archivo
            await Js.InvokeVoidAsync("blazorDownloadFile", fileName,
           contentBytes);

        }

    }
}
