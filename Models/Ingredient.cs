using Postgrest.Attributes;
using Postgrest.Models;

namespace KitchenBuddyAPI.Models;

[Table("ingredients")]
public class Ingredient : BaseModel
{
    [Column("recipe_id")]
    public long RecipeId {get; set;}
    [PrimaryKey("ingredient_id", false)]
    public long IngredientId { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("amount")]
    public double Amount { get; set; }
    [Column("unit")]
    // public string Unit { get; set; }
    public MeassureUnit Unit { get; set; }
}