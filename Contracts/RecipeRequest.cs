namespace KitchenBuddyAPI.Contracts;

public record RecipeRequest(long Id, DateTime CreatedAt, string Title, string Description, int CreatedBy);