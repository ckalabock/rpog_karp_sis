using BiblApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblApp.Data;

public class LibraryContext : DbContext
{
    public const string ConnectionString = "Host=localhost;Port=5432;Database=bibl_library;Username=postgres;Password=030609";

    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.BirthDate)
                .HasColumnType("date");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("genres");
            entity.HasKey(g => g.Id);

            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(g => g.Description)
                .HasMaxLength(500);

            entity.HasIndex(g => g.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books", table =>
            {
                table.HasCheckConstraint("CK_Book_PublishYear", "\"PublishYear\" >= 1000 AND \"PublishYear\" <= 3000");
                table.HasCheckConstraint("CK_Book_QuantityInStock", "\"QuantityInStock\" >= 0");
            });
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(b => b.PublishYear)
                .IsRequired();

            entity.Property(b => b.QuantityInStock)
                .IsRequired();

            entity.HasIndex(b => b.ISBN)
                .IsUnique();

            entity.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
