using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;
    [SerializeField] private float chanceToLooseEquipments;

    public override void GenerateDrop() {

        Inventory inventory = Inventory.instance;


        List<InventoryItem> currentEquipment = inventory.GetEquipmentList();
        List<InventoryItem> currentStash = inventory.GetStashList();
        List<InventoryItem> currentInventory = inventory.GetInventoryList();

        List<InventoryItem> equipmentCopy = new List<InventoryItem>(currentEquipment);
        List<InventoryItem> stashCopy = new List<InventoryItem>(currentStash);
        List<InventoryItem> inventoryCopy = new List<InventoryItem>(currentInventory);

        foreach (InventoryItem item in equipmentCopy) {
            if (Random.Range(0, 100) <= chanceToLooseEquipments) {
                DropItem(item.data);
                inventory.UnequipItem(item.data as ItemData_Equipment);
                inventory.UpdateSlotUI();
            }
        }

        foreach(InventoryItem item in inventoryCopy) {
            if (Random.Range(0, 100) <= chanceToLooseEquipments) {
                DropItem(item.data);
                inventory.RemoveItem(item.data);
                inventory.UpdateSlotUI();
            }
        }

        foreach (InventoryItem item in stashCopy) {
            if (Random.Range(0, 100) <= chanceToLooseMaterials) {
                DropItem(item.data);
                inventory.RemoveItem(item.data);
                inventory.UpdateSlotUI();
            }
        }
    }
}
