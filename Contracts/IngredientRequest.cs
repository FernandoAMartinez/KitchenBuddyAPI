namespace KitchenBuddyAPI.Contracts;

// public record IngredientRequest(long RecipeId, long IngredientId, string Name, double Amount, string Unit);
public record IngredientRequest(long RecipeId, long IngredientId, string Name, double Amount, Models.MeassureUnit Unit);