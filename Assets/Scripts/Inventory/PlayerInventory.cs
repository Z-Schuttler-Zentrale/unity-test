using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(Item itemData)
    {
        InventoryItem existingItem = items.Find(i => i.data == itemData);
        if (existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            items.Add(new InventoryItem(itemData));
        }

        Debug.Log($"Item hinzugefügt: {itemData.itemName}");
    }

    public void UseItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            InventoryItem item = items[index];
            if (item.data.prefab != null)
            {
                Instantiate(item.data.prefab, transform.position + transform.forward * 2f, Quaternion.identity);
                Debug.Log($"Item verwendet: {item.data.itemName}");
            }
        }
    }
}
