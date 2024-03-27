namespace KitchenBuddyAPI.Contracts;
// public record IngredientResponse(long RecipeId, long IngredientId, string Name, double Amount, string Unit);
public record IngredientResponse(long RecipeId, long IngredientId, string Name, double Amount, Models.MeassureUnit Unit);