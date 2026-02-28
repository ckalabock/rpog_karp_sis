namespace BiblApp.ViewModels;

public class FilterOption
{
    public int? Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public override string ToString() => Name;
}
