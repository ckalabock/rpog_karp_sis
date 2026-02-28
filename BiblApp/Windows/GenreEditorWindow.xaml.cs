using System.Windows;
using BiblApp.Data;
using BiblApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblApp.Windows;

public partial class GenreEditorWindow : Window
{
    private readonly int? _genreId;

    public GenreEditorWindow(int? genreId = null)
    {
        InitializeComponent();
        _genreId = genreId;

        if (_genreId.HasValue)
        {
            Title = "Edit genre";
            LoadGenre(_genreId.Value);
        }
        else
        {
            Title = "Add genre";
        }
    }

    private void LoadGenre(int id)
    {
        using var db = new LibraryContext();
        var genre = db.Genres.AsNoTracking().FirstOrDefault(g => g.Id == id);

        if (genre is null)
        {
            MessageBox.Show("Genre not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        NameTextBox.Text = genre.Name;
        DescriptionTextBox.Text = genre.Description;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInputs())
        {
            return;
        }

        using var db = new LibraryContext();

        var genre = _genreId.HasValue
            ? db.Genres.FirstOrDefault(g => g.Id == _genreId.Value)
            : new Genre();

        if (genre is null)
        {
            MessageBox.Show("Genre not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        genre.Name = NameTextBox.Text.Trim();
        genre.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
            ? null
            : DescriptionTextBox.Text.Trim();

        if (!_genreId.HasValue)
        {
            db.Genres.Add(genre);
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

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }
}
