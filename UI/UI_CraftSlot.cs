using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{

    protected override void Start() {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data) {
        if(_data == null) {
            return;
        }
        item.data = _data;

        itemImage.sprite = _data.icon;
        itemText.text = _data.name;
    }
    private void OnEnable() {
        UpdateSlot(item);
    }
    public override void OnPointerDown(PointerEventData eventData) {
        ui.ui_CraftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}
