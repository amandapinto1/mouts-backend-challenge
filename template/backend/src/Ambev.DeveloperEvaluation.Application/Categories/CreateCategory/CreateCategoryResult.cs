namespace Ambev.DeveloperEvaluation.Application.Categories.CreateCategory;

public class CreateCategoryResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
