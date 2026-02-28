using System.Windows;
using BiblApp.Data;
using BiblApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblApp.Windows;

public partial class AuthorEditorWindow : Window
{
    private readonly int? _authorId;

    public AuthorEditorWindow(int? authorId = null)
    {
        InitializeComponent();
        _authorId = authorId;

        if (_authorId.HasValue)
        {
            Title = "Edit author";
            LoadAuthor(_authorId.Value);
        }
        else
        {
            Title = "Add author";
            BirthDatePicker.SelectedDate = new DateTime(1980, 1, 1);
        }
    }

    private void LoadAuthor(int id)
    {
        using var db = new LibraryContext();
        var author = db.Authors.AsNoTracking().FirstOrDefault(a => a.Id == id);

        if (author is null)
        {
            MessageBox.Show("Author not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        FirstNameTextBox.Text = author.FirstName;
        LastNameTextBox.Text = author.LastName;
        BirthDatePicker.SelectedDate = author.BirthDate;
        CountryTextBox.Text = author.Country;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInputs(out var birthDate))
        {
            return;
        }

        using var db = new LibraryContext();

        var author = _authorId.HasValue
            ? db.Authors.FirstOrDefault(a => a.Id == _authorId.Value)
            : new Author();

        if (author is null)
        {
            MessageBox.Show("Author not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        author.FirstName = FirstNameTextBox.Text.Trim();
        author.LastName = LastNameTextBox.Text.Trim();
        author.BirthDate = birthDate;
        author.Country = CountryTextBox.Text.Trim();

        if (!_authorId.HasValue)
        {
            db.Authors.Add(author);
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

    private bool ValidateInputs(out DateTime birthDate)
    {
        birthDate = default;

        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
        {
            MessageBox.Show("First name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Last name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (BirthDatePicker.SelectedDate is null)
        {
            MessageBox.Show("Birth date is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(CountryTextBox.Text))
        {
            MessageBox.Show("Country is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        birthDate = BirthDatePicker.SelectedDate.Value.Date;
        return true;
    }
}
