using Postgrest.Attributes;
using Postgrest.Models;

namespace KitchenBuddyAPI.Models;

[Table("followers")]
public class FollowProfile : BaseModel
{
    [Column("profile_id")]
    public int ProfileId { get; set; }
    [Column("follower_id")]
    public int FollowerId { get; set; }
}
