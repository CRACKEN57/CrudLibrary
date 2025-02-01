using CrudTodos.Data.Models;
using Microsoft.AspNetCore.Components;

namespace CrudTodos.Presentations.Components
{
    public partial class ComponentListBooks
    {
        private List<Book> filteredBooks = new();
        private List<Author> Authors = new();
        private string searchTerm { get; set; } = string.Empty;
        private IEnumerable<Book> books;
        private Book selectedBook = new Book();

        private int currentPage = 1;
        private int pageSize = 5;  // Por ejemplo, 5 libros por página
        private int totalPages => (int)Math.Ceiling((double)filteredBooks.Count / pageSize);

        private System.Timers.Timer debounceTimer;

        protected override async Task OnInitializedAsync()
        {
            books = await BookService.GetBooksAsync();
            filteredBooks = books?.ToList() ?? new List<Book>();
            Authors = (await AuthorService.GetAllAuthorAsync()).ToList();
        }

        private void FilterBooks()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredBooks = books.ToList();
            }
            else
            {
                filteredBooks = books
                    .Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                b.Id_Book.ToString().Contains(searchTerm))
                    .ToList();
            }

            InvokeAsync(StateHasChanged);
        }

        private void DebounceFilterCustomers(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;

            debounceTimer?.Stop();
            debounceTimer = new System.Timers.Timer(300); // 300 ms de espera antes de ejecutar
            debounceTimer.Elapsed += (_, _) =>
            {
                debounceTimer.Stop();
                FilterBooks();
            };
            debounceTimer.Start();
        }

        private async Task DeleteBook(string Id_Book)
        {
            try
            {
                await BookService.DeleteBookAsync(Id_Book);
                books = await BookService.GetBooksAsync();
                StateHasChanged();
                Navigation.NavigateTo("/ListBooks", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar libro: {ex.Message}");
            }
        }

        private async Task GuardarCambios()
        {
            if (selectedBook == null)
            {
                Console.WriteLine("No hay libro seleccionado.");
                return;
            }

            try
            {
                await BookService.UpdateBookAsync(selectedBook);
                Console.WriteLine("Libro actualizado correctamente.");

                StateHasChanged();

                // Pequeño delay para evitar problemas de renderizado
                await Task.Delay(200);

                // Redirigir
                Navigation.NavigateTo("/ListBooks", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar libro: {ex.Message}");
            }
        }

        private List<Book> PaginateBooks()
        {
            return filteredBooks
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private void OpenEditModal(Book book)
        {
            selectedBook = book;
            StateHasChanged();
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
    }
}
