namespace KitchenBuddyAPI.Contracts;
public record RecipeResponse(long Id, DateTime CreatedAt, string Title, string Description, long CreatedBy, string Base, string Origin, string[] Steps);