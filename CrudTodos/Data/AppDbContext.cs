using CrudTodos.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudTodos.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        // Dbset representa la collecion de todas las entidades del contexto 
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(a => a.Author)
                .WithMany(b => b.Books)
                .HasForeignKey(c => c.Author_id);
        }
    }
}
