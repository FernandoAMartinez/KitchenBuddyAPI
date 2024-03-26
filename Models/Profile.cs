using Postgrest.Attributes;
using Postgrest.Models;

namespace KitchenBuddyAPI.Models;

//Table: Profiles
[Table("profiles")]
public class Profile : BaseModel
{
    //[PrimaryKey("id", false)]
    [PrimaryKey("id", false)]
    public long Id { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("email")]
    public string Email { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; }
    [Column("last_name")]
    public string LastName { get; set; }
    [Column("birthday")]
    public DateTime Birthday { get; set; }
    [Column("follower_count")]
    public int FollowerCount { get; set; }
    [Column("following_count")]
    public int FollowingCount { get; set; }
}
