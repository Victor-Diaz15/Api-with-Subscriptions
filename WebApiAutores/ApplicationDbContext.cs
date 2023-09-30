using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entiities;

namespace WebApiAutores
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AutorLibro>()
                .HasKey(al => new {al.AutorId, al.LibroId});

            //Aplicandole un tipo en especifico de tamaño al decimal de monto de la entidad de factura
            modelBuilder.Entity<Factura>()
                .Property(x => x.Monto).HasColumnType("decimal(18,2)");
        }

        #region DbSets
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set; }
        public DbSet<LlaveApi> LlavesApi { get; set; }
        public DbSet<Peticion> Peticiones { get; set; }
        public DbSet<RestriccionDominio> RestriccionesDominio { get; set; }
        public DbSet<RestriccionIP> RestriccionesIP { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaEmitida> FacturasEmitidas { get; set; }

        #endregion
    }
}
