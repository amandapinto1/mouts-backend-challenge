namespace Ambev.DeveloperEvaluation.Application.Categories.ListCategories;

public class ListCategoriesResult
{
    public List<ListCategoriesItemResult> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class ListCategoriesItemResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
