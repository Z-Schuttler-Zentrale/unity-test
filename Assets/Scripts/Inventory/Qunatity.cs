

[System.Serializable]
public class InventoryItem
{
    public Item data;
    public int quantity;

    public InventoryItem(Item data, int quantity = 1)
    {
        this.data = data;
        this.quantity = quantity;
    }
}
