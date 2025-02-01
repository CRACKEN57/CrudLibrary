using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrudTodos.Data.Models
{
    public class Author
    {
        [Key]
        [Required(ErrorMessage = "El ID del autor es obligatorio.")]
        public string Author_Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(1, 120, ErrorMessage = "La edad debe ser un número positivo mayor a 0.")]
        public int Age { get; set; }

        [MaxLength(500, ErrorMessage = "La biografía no puede tener más de 500 caracteres.")]
        public string Biography { get; set; }

        public bool Status { get; set; } = true;

        //Relacion con libros
        public ICollection<Book> Books { get; set; }

        

    }
}
