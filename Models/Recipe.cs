using Postgrest.Attributes;
using Postgrest.Models;

namespace KitchenBuddyAPI.Models;

[Table("recipes")]
public class Recipe : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("title")]
    public string Title { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [Column("base")]
    public string Base { get; set; }
    [Column("origin")]
    public string Origin { get; set; }
    [Column("created_by")]
    public int CreatedBy { get; set; } //Foreign Key to User.Id
    [Column("ingredients")]
    public List<Ingredient> Ingredients { get; set; }
    [Column("steps")]
    public string[] Steps { get; set; }
}