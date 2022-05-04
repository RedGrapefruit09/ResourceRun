public class ItemStack
{
    public int Amount { get; set; }
    public Item Item { get; }

    public ItemStack(Item item, int amount = 1)
    {
        Item = item;
        Amount = amount;
    }

    public void Increment()
    {
        if (Amount < Item.maxCount)
        {
            Amount++;
        }
    }

    public void Decrement()
    {
        Amount--;
    }
}