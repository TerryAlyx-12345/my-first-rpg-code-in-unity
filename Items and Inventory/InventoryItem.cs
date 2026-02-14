using System;
using UnityEngine;

[Serializable]
public class InventoryItem {
    public ItemData data;
    public int stackSize;
    public InventoryItem(ItemData _newItemData) {
        data = _newItemData;
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
