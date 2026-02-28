using System.Windows;
using System.Windows.Controls;
using BiblApp.Data;
using BiblApp.Models;
using BiblApp.ViewModels;
using BiblApp.Windows;
using Microsoft.EntityFrameworkCore;

namespace BiblApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadFilters();
        LoadBooks();
    }

    private Book? SelectedBook => BooksDataGrid.SelectedItem as Book;

    private void LoadFilters()
    {
        using var db = new LibraryContext();

        var selectedAuthorId = (AuthorFilterComboBox.SelectedItem as FilterOption)?.Id;
        var selectedGenreId = (GenreFilterComboBox.SelectedItem as FilterOption)?.Id;

        var authorFilters = new List<FilterOption>
        {
            new() { Id = null, Name = "All" }
        };

        authorFilters.AddRange(db.Authors
            .AsNoTracking()
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .Select(a => new FilterOption { Id = a.Id, Name = a.LastName + " " + a.FirstName })
            .ToList());

        var genreFilters = new List<FilterOption>
        {
            new() { Id = null, Name = "All" }
        };

        genreFilters.AddRange(db.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => new FilterOption { Id = g.Id, Name = g.Name })
            .ToList());

        AuthorFilterComboBox.ItemsSource = authorFilters;
        GenreFilterComboBox.ItemsSource = genreFilters;

        AuthorFilterComboBox.SelectedItem = authorFilters.FirstOrDefault(a => a.Id == selectedAuthorId) ?? authorFilters[0];
        GenreFilterComboBox.SelectedItem = genreFilters.FirstOrDefault(g => g.Id == selectedGenreId) ?? genreFilters[0];
    }

    private void LoadBooks()
    {
        using var db = new LibraryContext();

        var query = db.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .AsQueryable();

        var search = SearchTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => EF.Functions.ILike(b.Title, $"%{search}%"));
        }

        var authorFilter = (AuthorFilterComboBox.SelectedItem as FilterOption)?.Id;
        if (authorFilter.HasValue)
        {
            query = query.Where(b => b.AuthorId == authorFilter.Value);
        }

        var genreFilter = (GenreFilterComboBox.SelectedItem as FilterOption)?.Id;
        if (genreFilter.HasValue)
        {
            query = query.Where(b => b.GenreId == genreFilter.Value);
        }

        BooksDataGrid.ItemsSource = query
            .OrderBy(b => b.Title)
            .ToList();
    }

    private void FilterChanged(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        LoadBooks();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadFilters();
        LoadBooks();
    }

    private void AddBookButton_Click(object sender, RoutedEventArgs e)
    {
        if (!HasAnyAuthorAndGenre())
        {
            return;
        }

        var editor = new BookEditorWindow { Owner = this };
        if (editor.ShowDialog() == true)
        {
            LoadBooks();
        }
    }

    private void EditBookButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedBook is null)
        {
            MessageBox.Show("Select a book first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var editor = new BookEditorWindow(SelectedBook.Id) { Owner = this };
        if (editor.ShowDialog() == true)
        {
            LoadBooks();
        }
    }

    private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedBook is null)
        {
            MessageBox.Show("Select a book first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Delete '{SelectedBook.Title}'?",
            "Confirm delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        using var db = new LibraryContext();
        var book = db.Books.FirstOrDefault(b => b.Id == SelectedBook.Id);

        if (book is null)
        {
            MessageBox.Show("Book not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        db.Books.Remove(book);
        db.SaveChanges();
        LoadBooks();
    }

    private void ManageAuthorsButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new AuthorManagementWindow { Owner = this };
        window.ShowDialog();
        LoadFilters();
        LoadBooks();
    }

    private void ManageGenresButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new GenreManagementWindow { Owner = this };
        window.ShowDialog();
        LoadFilters();
        LoadBooks();
    }

    private bool HasAnyAuthorAndGenre()
    {
        using var db = new LibraryContext();
        var hasAuthor = db.Authors.Any();
        var hasGenre = db.Genres.Any();

        if (hasAuthor && hasGenre)
        {
            return true;
        }

        MessageBox.Show(
            "At least one author and one genre must exist before adding a book.",
            "Cannot add book",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);

        return false;
    }
}
