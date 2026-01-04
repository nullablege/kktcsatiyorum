namespace KKTCSatiyorum.Models.Home;

public sealed class CategoryNavItemViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int SiraNo { get; init; }
    public List<CategoryNavItemViewModel> Children { get; init; } = new();
}
