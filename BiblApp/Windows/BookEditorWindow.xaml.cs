using System.Windows;
using BiblApp.Data;
using BiblApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblApp.Windows;

public partial class BookEditorWindow : Window
{
    private readonly int? _bookId;

    public BookEditorWindow(int? bookId = null)
    {
        InitializeComponent();
        _bookId = bookId;

        LoadLookups();

        if (_bookId.HasValue)
        {
            Title = "Edit book";
            LoadBook(_bookId.Value);
        }
        else
        {
            Title = "Add book";
            PublishYearTextBox.Text = DateTime.Now.Year.ToString();
            QuantityTextBox.Text = "0";
        }
    }

    private void LoadLookups()
    {
        using var db = new LibraryContext();

        var authors = db.Authors
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToList();

        var genres = db.Genres
            .OrderBy(g => g.Name)
            .ToList();

        AuthorComboBox.ItemsSource = authors;
        GenreComboBox.ItemsSource = genres;

        if (authors.Count > 0)
        {
            AuthorComboBox.SelectedIndex = 0;
        }

        if (genres.Count > 0)
        {
            GenreComboBox.SelectedIndex = 0;
        }
    }

    private void LoadBook(int id)
    {
        using var db = new LibraryContext();
        var book = db.Books.AsNoTracking().FirstOrDefault(b => b.Id == id);

        if (book is null)
        {
            MessageBox.Show("Book not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        TitleTextBox.Text = book.Title;
        PublishYearTextBox.Text = book.PublishYear.ToString();
        IsbnTextBox.Text = book.ISBN;
        QuantityTextBox.Text = book.QuantityInStock.ToString();

        SelectById(AuthorComboBox, book.AuthorId);
        SelectById(GenreComboBox, book.GenreId);
    }

    private static void SelectById(System.Windows.Controls.ComboBox comboBox, int id)
    {
        foreach (var item in comboBox.Items)
        {
            if (item is Author author && author.Id == id)
            {
                comboBox.SelectedItem = author;
                return;
            }

            if (item is Genre genre && genre.Id == id)
            {
                comboBox.SelectedItem = genre;
                return;
            }
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInputs(out var publishYear, out var quantity, out var author, out var genre))
        {
            return;
        }

        using var db = new LibraryContext();

        var book = _bookId.HasValue
            ? db.Books.FirstOrDefault(b => b.Id == _bookId.Value)
            : new Book();

        if (book is null)
        {
            MessageBox.Show("Book not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        book.Title = TitleTextBox.Text.Trim();
        book.AuthorId = author.Id;
        book.GenreId = genre.Id;
        book.PublishYear = publishYear;
        book.ISBN = IsbnTextBox.Text.Trim();
        book.QuantityInStock = quantity;

        if (!_bookId.HasValue)
        {
            db.Books.Add(book);
        }

        try
        {
            db.SaveChanges();
            DialogResult = true;
            Close();
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Save error: {ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateInputs(out int publishYear, out int quantity, out Author author, out Genre genre)
    {
        publishYear = 0;
        quantity = 0;
        author = null!;
        genre = null!;

        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("Title is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!int.TryParse(PublishYearTextBox.Text.Trim(), out publishYear) || publishYear < 1000 || publishYear > 3000)
        {
            MessageBox.Show("Publish year must be between 1000 and 3000.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(IsbnTextBox.Text))
        {
            MessageBox.Show("ISBN is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!int.TryParse(QuantityTextBox.Text.Trim(), out quantity) || quantity < 0)
        {
            MessageBox.Show("Quantity must be a non-negative number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (AuthorComboBox.SelectedItem is not Author selectedAuthor)
        {
            MessageBox.Show("Select an author.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (GenreComboBox.SelectedItem is not Genre selectedGenre)
        {
            MessageBox.Show("Select a genre.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        author = selectedAuthor;
        genre = selectedGenre;
        return true;
    }
}
