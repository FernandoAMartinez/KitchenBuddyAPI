using Postgrest.Attributes;
using Postgrest.Models;

namespace KitchenBuddyAPI.Models;

//Table: Storages
[Table("Storages")]
public class Storage : BaseModel
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public string Name { get; set; }
    public Ingredient Products { get; set; }
}
