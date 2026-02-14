using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class UI_EquipmentSlot : UI_ItemSlot
{ 
    public EquipmentType slotType;


    protected override void Start() {
        base.Start(); 
    }



    private void OnValidate() {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData) {
        if(item == null || item.data == null) {
            return;
        }
        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);

        ui.ui_ItemTooltip.HideToolTip();
        CleanUpSlot();
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        if (item == null) {
            return;
        }
        ui.ui_ItemTooltip.ShowToolTip(item.data as ItemData_Equipment);
    }
}
