using Microsoft.AspNetCore.Mvc;

namespace ExpenseAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
    /// <summary>
    /// Get all available expense categories
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<CategoryResponse>> GetCategories()
    {
        var categories = ExpenseAI.Domain.ValueObjects.Category.SystemCategories
            .Select(c => new CategoryResponse(
                c.Name,
                c.Description ?? string.Empty,
                c.Color,
                c.Icon,
                c.IsSystemCategory))
            .ToList();

        return Ok(categories);
    }

    /// <summary>
    /// Get a specific category by name
    /// </summary>
    [HttpGet("{name}")]
    public ActionResult<CategoryResponse> GetCategory(string name)
    {
        var category = ExpenseAI.Domain.ValueObjects.Category.SystemCategories
            .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (category == null)
            return NotFound($"Category '{name}' not found");

        var response = new CategoryResponse(
            category.Name,
            category.Description ?? string.Empty,
            category.Color,
            category.Icon,
            category.IsSystemCategory);

        return Ok(response);
    }
}

public record CategoryResponse(
    string Name,
    string Description,
    string Color,
    string Icon,
    bool IsSystemCategory);
