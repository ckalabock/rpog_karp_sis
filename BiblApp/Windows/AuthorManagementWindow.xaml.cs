using System.Windows;
using BiblApp.Data;
using BiblApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblApp.Windows;

public partial class AuthorManagementWindow : Window
{
    public AuthorManagementWindow()
    {
        InitializeComponent();
        LoadAuthors();
    }

    private void LoadAuthors()
    {
        using var db = new LibraryContext();
        AuthorsDataGrid.ItemsSource = db.Authors
            .AsNoTracking()
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToList();
    }

    private Author? SelectedAuthor => AuthorsDataGrid.SelectedItem as Author;

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var editor = new AuthorEditorWindow { Owner = this };
        if (editor.ShowDialog() == true)
        {
            LoadAuthors();
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedAuthor is null)
        {
            MessageBox.Show("Select an author first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var editor = new AuthorEditorWindow(SelectedAuthor.Id) { Owner = this };
        if (editor.ShowDialog() == true)
        {
            LoadAuthors();
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedAuthor is null)
        {
            MessageBox.Show("Select an author first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirm = MessageBox.Show(
            "Delete selected author? All related books will be removed.",
            "Confirm delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes)
        {
            return;
        }

        using var db = new LibraryContext();
        var author = db.Authors.FirstOrDefault(a => a.Id == SelectedAuthor.Id);

        if (author is null)
        {
            MessageBox.Show("Author not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        db.Authors.Remove(author);

        try
        {
            db.SaveChanges();
            LoadAuthors();
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
