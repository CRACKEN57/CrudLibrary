using CrudTodos.Data.Models;

namespace CrudTodos.Presentations.Components
{
    public partial class ComponentBook
    {
        private Book NuevoLibro = new();
        private string Mensaje;
        private List<Author> Authors = new();

        protected override async Task OnInitializedAsync()
        {
            NuevoLibro.Edition_date = DateTime.Now;
            Authors = (await AuthorService.GetAllAuthorAsync()).ToList();
        }

        private async Task SaveBook()
        {
            var ExistingBook = await BookService.IsBookUniqueAsync(NuevoLibro.Id_Book);

            if (ExistingBook)
            {
                try
                {
                    await BookService.AddBookAsync(NuevoLibro);
                    NuevoLibro = new Book { Edition_date = DateTime.Now };
                    Navigate.NavigateTo("ListBooks", true);
                }
                catch (Exception ex)
                {
                    Mensaje = $"Error al guardar el libro: {ex.Message}";
                }
            }
            else
            {
                Mensaje = "El libro ya existe";
            }

        }
    }
}
