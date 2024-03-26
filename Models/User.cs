using Postgrest.Attributes;
using Postgrest.Models;

namespace KitchenBuddyAPI.Models;

[Table("users")]
public class User : BaseModel
{
    //[PrimaryKey("id", false)]
    [PrimaryKey("id", false)]
    public long Id { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("email")]
    public string Email { get; set; }
    [Column("password")]
    public string Password { get; set; }
}
