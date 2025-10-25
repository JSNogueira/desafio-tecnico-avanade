using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Context
{
    public class UsuarioContext : DbContext
    {
        public UsuarioContext(DbContextOptions<UsuarioContext> options) : base(options)
        {
            
        }

        public DbSet<Usuario> Usuarios { get; set; }

        // Faz com que o Enum seja criado como string ao invés de int no banco
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .Property(u => u.TipoUsuario) 
                .HasConversion<string>()
                .HasMaxLength(20); 

            base.OnModelCreating(modelBuilder);
        }
    }
}