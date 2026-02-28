using BiblApp.Models;

namespace BiblApp.Data;

public static class SeedData
{
    public static void EnsureSeeded(LibraryContext context)
    {
        var legacyTitles = new[] { "1984", "To Kill a Mockingbird" };
        if (context.Books.Any(b => legacyTitles.Contains(b.Title)))
        {
            context.Books.RemoveRange(context.Books);
            context.Authors.RemoveRange(context.Authors);
            context.Genres.RemoveRange(context.Genres);
            context.SaveChanges();
        }

        if (context.Books.Any())
        {
            return;
        }

        var authors = new[]
        {
            new Author { FirstName = "Jane", LastName = "Austen", BirthDate = new DateTime(1775, 12, 16), Country = "United Kingdom" },
            new Author { FirstName = "Herman", LastName = "Melville", BirthDate = new DateTime(1819, 8, 1), Country = "United States" },
            new Author { FirstName = "F. Scott", LastName = "Fitzgerald", BirthDate = new DateTime(1896, 9, 24), Country = "United States" },
            new Author { FirstName = "Leo", LastName = "Tolstoy", BirthDate = new DateTime(1828, 9, 9), Country = "Russia" },
            new Author { FirstName = "Fyodor", LastName = "Dostoevsky", BirthDate = new DateTime(1821, 11, 11), Country = "Russia" },
            new Author { FirstName = "J.D.", LastName = "Salinger", BirthDate = new DateTime(1919, 1, 1), Country = "United States" },
            new Author { FirstName = "Aldous", LastName = "Huxley", BirthDate = new DateTime(1894, 7, 26), Country = "United Kingdom" },
            new Author { FirstName = "J.R.R.", LastName = "Tolkien", BirthDate = new DateTime(1892, 1, 3), Country = "United Kingdom" },
            new Author { FirstName = "Ray", LastName = "Bradbury", BirthDate = new DateTime(1920, 8, 22), Country = "United States" },
            new Author { FirstName = "Charlotte", LastName = "Bronte", BirthDate = new DateTime(1816, 4, 21), Country = "United Kingdom" },
            new Author { FirstName = "George", LastName = "Orwell", BirthDate = new DateTime(1903, 6, 25), Country = "United Kingdom" },
            new Author { FirstName = "Oscar", LastName = "Wilde", BirthDate = new DateTime(1854, 10, 16), Country = "Ireland" },
            new Author { FirstName = "Gabriel Garcia", LastName = "Marquez", BirthDate = new DateTime(1927, 3, 6), Country = "Colombia" },
            new Author { FirstName = "Paulo", LastName = "Coelho", BirthDate = new DateTime(1947, 8, 24), Country = "Brazil" }
        };

        var genres = new[]
        {
            new Genre { Name = "Classic", Description = "Classic literature" },
            new Genre { Name = "Fantasy", Description = "Fantasy works" },
            new Genre { Name = "Dystopia", Description = "Dystopian novels" },
            new Genre { Name = "Science Fiction", Description = "Science fiction works" },
            new Genre { Name = "Historical Novel", Description = "Historical fiction" },
            new Genre { Name = "Magical Realism", Description = "Magical realism" }
        };

        context.Authors.AddRange(authors);
        context.Genres.AddRange(genres);
        context.SaveChanges();

        Author A(string lastName) => context.Authors.First(a => a.LastName == lastName);
        Genre G(string name) => context.Genres.First(g => g.Name == name);

        var books = new[]
        {
            new Book { Title = "Pride and Prejudice", AuthorId = A("Austen").Id, GenreId = G("Classic").Id, PublishYear = 1813, ISBN = "9780141439518", QuantityInStock = 5 },
            new Book { Title = "Moby-Dick", AuthorId = A("Melville").Id, GenreId = G("Classic").Id, PublishYear = 1851, ISBN = "9780142437247", QuantityInStock = 3 },
            new Book { Title = "The Great Gatsby", AuthorId = A("Fitzgerald").Id, GenreId = G("Classic").Id, PublishYear = 1925, ISBN = "9780743273565", QuantityInStock = 6 },
            new Book { Title = "War and Peace", AuthorId = A("Tolstoy").Id, GenreId = G("Historical Novel").Id, PublishYear = 1869, ISBN = "9780199232765", QuantityInStock = 4 },
            new Book { Title = "Crime and Punishment", AuthorId = A("Dostoevsky").Id, GenreId = G("Classic").Id, PublishYear = 1866, ISBN = "9780140449136", QuantityInStock = 7 },
            new Book { Title = "The Catcher in the Rye", AuthorId = A("Salinger").Id, GenreId = G("Classic").Id, PublishYear = 1951, ISBN = "9780316769488", QuantityInStock = 3 },
            new Book { Title = "Brave New World", AuthorId = A("Huxley").Id, GenreId = G("Dystopia").Id, PublishYear = 1932, ISBN = "9780060850524", QuantityInStock = 4 },
            new Book { Title = "The Hobbit", AuthorId = A("Tolkien").Id, GenreId = G("Fantasy").Id, PublishYear = 1937, ISBN = "9780547928227", QuantityInStock = 8 },
            new Book { Title = "Fahrenheit 451", AuthorId = A("Bradbury").Id, GenreId = G("Science Fiction").Id, PublishYear = 1953, ISBN = "9781451673319", QuantityInStock = 5 },
            new Book { Title = "The Lord of the Rings", AuthorId = A("Tolkien").Id, GenreId = G("Fantasy").Id, PublishYear = 1954, ISBN = "9780544003415", QuantityInStock = 2 },
            new Book { Title = "Jane Eyre", AuthorId = A("Bronte").Id, GenreId = G("Classic").Id, PublishYear = 1847, ISBN = "9780141441146", QuantityInStock = 5 },
            new Book { Title = "Animal Farm", AuthorId = A("Orwell").Id, GenreId = G("Dystopia").Id, PublishYear = 1945, ISBN = "9780451526342", QuantityInStock = 6 },
            new Book { Title = "The Picture of Dorian Gray", AuthorId = A("Wilde").Id, GenreId = G("Classic").Id, PublishYear = 1890, ISBN = "9780141439570", QuantityInStock = 3 },
            new Book { Title = "One Hundred Years of Solitude", AuthorId = A("Marquez").Id, GenreId = G("Magical Realism").Id, PublishYear = 1967, ISBN = "9780060883287", QuantityInStock = 4 },
            new Book { Title = "The Alchemist", AuthorId = A("Coelho").Id, GenreId = G("Classic").Id, PublishYear = 1988, ISBN = "9780061122415", QuantityInStock = 9 }
        };

        context.Books.AddRange(books);
        context.SaveChanges();
    }
}
