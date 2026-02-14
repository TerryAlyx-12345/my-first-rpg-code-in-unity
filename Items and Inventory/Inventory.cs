using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour ,ISaveManager
{

    public static Inventory instance;

    public List<ItemData> startingEquipment;

    public List<InventoryItem> equipments;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary; 

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;


    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;
    
    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlots;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask = - Mathf.Infinity;
    private float lastTimeUseArmor = - Mathf.Infinity;


    public float flaskCooldown {  get;private set; }
    public float armorkCooldown {  get;private set; }


    [Header("Data base")]
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        SaveManager.instance.RegisterAndLoadData(this);
    }

    private void Start() {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();

        equipments = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();

        statSlots = statSlotParent.GetComponentsInChildren<UI_StatSlot>();
        AddStartingItems();
        UpdateSlotUI();
    }

    private void AddStartingItems() {
        foreach (ItemData_Equipment equipment in loadedEquipment) {
            EquipItem(equipment);
        }
        if(loadedItems.Count > 0) {
            foreach(InventoryItem item in loadedItems) {
                for (int i = 0; i < item.stackSize; i++) {
                    AddItem(item.data);
                }
            }
            return;
        }
        if (loadedItems.Count > 0 || loadedEquipment.Count > 0) {
            return;
        }
        for (int i = 0; i < startingEquipment.Count; i++) {
            if (startingEquipment[i] != null)
                AddItem(startingEquipment[i]);
        }
    }

    public void UpdateSlotUI() {
        for (int i = 0; i < equipmentSlot.Length; i++) {
            equipmentSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < equipmentSlot.Length; i++) {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary) {
                if (item.Key.equipmentType == equipmentSlot[i].slotType) {
                    equipmentSlot[i].UpdateSlot(item.Value);
                }
            }
        }
        for (int i = 0; i < inventoryItemSlot.Length; i++) {
            inventoryItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < stashItemSlot.Length; i++) {
            stashItemSlot[i].CleanUpSlot();
        }
        for (int i = 0; i < inventory.Count; i++) {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }
        for (int i = 0; i < stash.Count; i++) {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }
        UpdateStartUI();
    }

    public void UpdateStartUI() {
        for (int i = 0; i < statSlots.Length; i++) {
            statSlots[i].UpdateStatValueUI();
        }
    }

    public void EquipItem(ItemData _item) {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary) {
            if (item.Key.equipmentType == newEquipment.equipmentType) {
                oldEquipment = item.Key;
            }
        }
        if(oldEquipment != null) {
            AddItem(oldEquipment);
            UnequipItem(oldEquipment);
        }


        equipments.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();
        RemoveItem(_item);

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove) {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value)) {

            equipments.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    public void AddItem(ItemData _item) {
        if (_item.itemType == ItemType.Equipment && CanAddItem()) {
            AddToIventory(_item);
        }
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);
        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item) {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value)) {
            value.AddStack();
        }
        else {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
            newItem.AddStack();
        }
    }

    private void AddToIventory(ItemData _item) {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value)) {
            value.AddStack();
        }
        else {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
            newItem.AddStack();
        }

    }

    public void RemoveItem(ItemData _item) {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value)) {
            value.RemoveStack();
            if (value.stackSize <= 0) {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
        }
        if(stashDictionary.TryGetValue(_item, out InventoryItem stashValue)) {
            stashValue.RemoveStack();
            if (stashValue.stackSize <= 0) {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
        }
        UpdateSlotUI();
    } 
    public bool CanAddItem() {
        if(inventory.Count >= inventoryItemSlot.Length) {
            Debug.Log("no more space");
            return false;
        }
        return true;
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requireMaterials) {
        List<InventoryItem> materialsToUse = new List<InventoryItem>();
        for (int i = 0; i < _requireMaterials.Count; i++) {
            if (stashDictionary.TryGetValue(_requireMaterials[i].data,out InventoryItem stashValue)) {
                if(stashValue.stackSize < _requireMaterials[i].stackSize) {
                    Debug.Log("Cannot craft, missing materials");
                    return false;
                }
                else {
                    materialsToUse.Add(stashValue);
                }
            }
            else {
                Debug.Log("Cannot craft, missing materials");
                return false;
            }
        }
        for (int i = 0; i < materialsToUse.Count; i++) {
            for (int j = 0; j < _requireMaterials[i].stackSize; j++) {
                RemoveItem(materialsToUse[i].data);
            }
        }
        AddItem(_itemToCraft);
        return true;
    }

    public List<InventoryItem> GetEquipmentList() {
        return equipments;
    }

    public List<InventoryItem> GetStashList() {
        return stash;
    }

    public List<InventoryItem> GetInventoryList() {
        return inventory;
    }

    public ItemData_Equipment GetEquipment(EquipmentType _type) {

        ItemData_Equipment equipedItem = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary) {
            if (item.Key.equipmentType == _type) {
                equipedItem = item.Key;
            }
        }
        return equipedItem;
    }

    public void UseFlask() {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);
        if (currentFlask == null) {
            return;
        }
        flaskCooldown = currentFlask.itemCooldown;
        bool canUseFlask = Time.time > lastTimeUsedFlask + currentFlask.itemCooldown;

        if (canUseFlask) {
            currentFlask.ApplyItemEffects(null);
            lastTimeUsedFlask = Time.time;
        }
        else {
            Debug.Log("Flask On cooldown");
        }
    }
    public bool CanUseArmor() {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);
        if (Time.time > lastTimeUseArmor + currentArmor.itemCooldown) {
            lastTimeUseArmor = Time.time;
            return true;
        }
        Debug.Log("armor on cooldown");
        return false;
    }

    public void LoadData(GameData _data) {
        foreach(KeyValuePair<string,int> pair in _data.inventory) {
            foreach(var item in GetItemDatabase()) {
                if(item != null && item.itemID == pair.Key) {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }
        foreach(string loadedItemID in _data.equipmentID) {
            foreach(var item in GetItemDatabase()) {
                if(item != null && loadedItemID == item.itemID) {
                    ItemData_Equipment equipmentToLoad = item as ItemData_Equipment;
                    loadedEquipment.Add(equipmentToLoad);
                }
            }
        }
    }

    public void SaveData(ref GameData _data) {
        _data.inventory.Clear();
        _data.equipmentID.Clear();
        foreach(KeyValuePair<ItemData,InventoryItem> pair in inventoryDictionary) {
            _data.inventory.Add(pair.Key.itemID,pair.Value.stackSize);
        }
        foreach(KeyValuePair<ItemData,InventoryItem>pair in stashDictionary) {
            _data.inventory.Add(pair.Key.itemID,pair.Value.stackSize);
        }
        foreach(KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary) {
            _data.equipmentID.Add(pair.Key.itemID);
        }
    }
    private List<ItemData> GetItemDatabase() {
        List<ItemData> itemDatabase = new List<ItemData>();
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        itemDatabase.AddRange(allItems);
        return itemDatabase;
    }
    private void OnDestroy() {
        if (SaveManager.instance != null)
            SaveManager.instance.Unregister(this);
    }
}

