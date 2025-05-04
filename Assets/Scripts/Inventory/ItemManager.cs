using NUnit.Framework.Interfaces;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private Item itemData;

    void Start()
    {
        if (itemData != null && itemData.prefab != null)
        {
            Instantiate(itemData.prefab, transform.position, Quaternion.identity);
        }
    }
}