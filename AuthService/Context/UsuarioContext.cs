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

            // Lembrar de testar essa parte para criar um seed
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Administrador",
                    Email = "admin@mail.com",
                    Senha = "admin123",
                    TipoUsuario = TipoUsuarioEnum.Administrador
                },
                new Usuario
                {
                    Id = 2,
                    Nome = "Cliente1",
                    Email = "cliente1@mail.com",
                    Senha = "cliente123",
                    TipoUsuario = TipoUsuarioEnum.Cliente
                },
                new Usuario
                {
                    Id = 3,
                    Nome = "Cliente2",
                    Email = "cliente2@mail.com",
                    Senha = "cliente456",
                    TipoUsuario = TipoUsuarioEnum.Cliente
                }
            );
        }
    }
}