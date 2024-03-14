namespace KitchenBuddyAPI.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Amount { get; set; }
    public MeassureUnit Unit { get; set; }
}
