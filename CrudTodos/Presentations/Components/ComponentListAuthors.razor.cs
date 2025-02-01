using CrudTodos.Data.Models;
using Microsoft.AspNetCore.Components;

namespace CrudTodos.Presentations.Components
{
    public partial class ComponentListAuthors
    {
        private List<Author> filteredAuthors = new();
        private IEnumerable<Book> books;
        private string searchTerm { get; set; } = string.Empty;
        private IEnumerable<Author> authors;
        private Author selectedAuthor = new Author();

        // new
        private int currentPage = 1;
        private int pageSize = 3;  // Por ejemplo, 5 clientes por página
        private int totalPages => (int)Math.Ceiling((double)filteredAuthors.Count / pageSize);
        private int AuthorBook = 0;

        private System.Timers.Timer debounceTimer;

        protected override async Task OnInitializedAsync()
        {
            authors = await AuthorService.GetAllAuthorAsync();
            filteredAuthors = authors?.ToList() ?? new List<Author>();
            books = await BookService.GetBooksAsync();
        }

        // Método optimizado para buscar localmente sin recargar la base de datos
        private void FilterAuthors()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredAuthors = authors.ToList();
            }
            else
            {
                filteredAuthors = authors
                    .Where(a => a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                a.Author_Id.ToString().Contains(searchTerm))
                    .ToList();
            }

            InvokeAsync(StateHasChanged);
        }

        // Método con "debounce" para mejorar la búsqueda en tiempo real
        private void DebounceFilterAuthors(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;

            debounceTimer?.Stop();
            debounceTimer = new System.Timers.Timer(100); // 100 ms de espera antes de ejecutar
            debounceTimer.Elapsed += (_, _) =>
            {
                debounceTimer.Stop();
                FilterAuthors();
            };
            debounceTimer.Start();
        }

        private async Task DeleteAuthor(string Id_Author)
        {
            try
            {
                await AuthorService.DeleteAuthorAsync(Id_Author);
                authors = await AuthorService.GetAllAuthorAsync();
                Navigation.NavigateTo("ListAuthors", true);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar cliente: {ex.Message}");
            }
        }

        private async Task GuardarCambios()
        {
            if (selectedAuthor == null)
            {
                Console.WriteLine("No hay libro seleccionado.");
                return;
            }

            try
            {
                await AuthorService.UpdateAuthorAsync(selectedAuthor);
                Console.WriteLine("Libro actualizado correctamente.");

                // Forzar actualización del estado antes de navegar
                StateHasChanged();

                // Pequeño delay para evitar problemas de renderizado
                await Task.Delay(200);

                // Redirigir
                Navigation.NavigateTo("/ListAuthors", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar libro: {ex.Message}");
            }
        }

        private List<Author> PaginateAuthors()
        {
            return filteredAuthors
                .Skip((currentPage - 1) * pageSize)  // Saltar los libros de las páginas anteriores
                .Take(pageSize)  // Tomar solo el número de libors de la página actual
                .ToList();
        }

        private void OpenEditModal(Author author)
        {
            selectedAuthor = author;
            StateHasChanged(); // Forzar actualización del DOM
        }

        private void GoToPreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
            }
        }

        private void GoToNextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
            }
        }

        private List<Book> BooksPerAuthor(string authorId)
        {
            return books.Where(b => b.Author_id == authorId).ToList();
        }

    }
}
