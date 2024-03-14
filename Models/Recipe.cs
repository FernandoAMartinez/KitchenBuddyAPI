namespace KitchenBuddyAPI.Models;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CreatedBy { get; set; } //Foreign Key to User.Id
    public Dictionary<string, Ingredient> Steps { get; set; }
}
