using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewMagazine", menuName = "Inventory/Item/Magazine")]
    public class Magazine : Item
    {
        public int maxSize = 30;
        public int size = 30;
    }
}