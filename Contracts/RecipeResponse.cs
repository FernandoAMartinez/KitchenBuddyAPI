namespace KitchenBuddyAPI.Contracts;
public record RecipeResponse(long Id, DateTime CreatedAt, string Title, string Description, int CreatedBy, List<IngredientResponse> Ingredients, string[] Steps);