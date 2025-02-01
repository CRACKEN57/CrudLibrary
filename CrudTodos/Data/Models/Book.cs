using System.ComponentModel.DataAnnotations;

namespace CrudTodos.Data.Models
{
    public class Book
    {
        [Key]
        [Required(ErrorMessage = "El Codigo del libro es obligatorio.")]
        public string Id_Book { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El título no puede tener más de 150 caracteres.")]
        [MinLength(3, ErrorMessage = "El título debe tener al menos 3 caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El número de páginas es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de páginas debe ser mayor a 0.")]
        public int Pages { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un número positivo mayor a 0.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "La fecha de edición es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "Debe ser una fecha válida.")]
        public DateTime Edition_date { get; set; }

        // Relación con el autor
        [Required(ErrorMessage = "El ID del autor es obligatorio.")]
        public string Author_id { get; set; }

        [Required(ErrorMessage = "La cantidad de libros es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad de libros no puede ser negativa.")]
        public int AmountBooks { get; set; }

        public Author Author { get; set; }

    }
}
