namespace KitchenBuddyAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime Birthday { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
}