using System.Windows;
using BiblApp.Data;
using BiblApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblApp.Windows;

public partial class GenreManagementWindow : Window
{
    public GenreManagementWindow()
    {
        InitializeComponent();
        LoadGenres();
    }

    private void LoadGenres()
    {
        using var db = new LibraryContext();
        GenresDataGrid.ItemsSource = db.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToList();
    }

    private Genre? SelectedGenre => GenresDataGrid.SelectedItem as Genre;

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var editor = new GenreEditorWindow { Owner = this };
        if (editor.ShowDialog() == true)
        {
            LoadGenres();
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedGenre is null)
        {
            MessageBox.Show("Select a genre first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var editor = new GenreEditorWindow(SelectedGenre.Id) { Owner = this };
        if (editor.ShowDialog() == true)
        {
            LoadGenres();
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedGenre is null)
        {
            MessageBox.Show("Select a genre first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirm = MessageBox.Show(
            "Delete selected genre? All related books will be removed.",
            "Confirm delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        using var db = new LibraryContext();
        var genre = db.Genres.FirstOrDefault(g => g.Id == SelectedGenre.Id);

        if (genre is null)
        {
            MessageBox.Show("Genre not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        db.Genres.Remove(genre);

        try
        {
            db.SaveChanges();
            LoadGenres();
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Delete error: {ex.InnerException?.Message ?? ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
