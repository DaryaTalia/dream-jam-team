[System.Serializable]
public class ItemStack
{
    public BaseItem baseItem;
    public int quantity;

    public ItemStack(BaseItem baseItem, int quantity)
    {
        this.baseItem = baseItem;
        this.quantity = quantity;
    }
}