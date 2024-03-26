using Postgrest.Attributes;
using Postgrest.Models;

namespace KitchenBuddyAPI.Models;

//Table: Shopping List
[Table("shopping_list")]
public class ShoppingList : BaseModel
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public List<Ingredient> Ingredients  { get; set; }
    public bool Bought { get; set; }
}