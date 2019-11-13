using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventoryManager : Singltone<InventoryManager>
{
    public Inventory inventory;
    [SerializeField] private EquipmentPanel equipmentPanel;

    private void Awake()
    {
        inventory.onItemRightClickedEvent += equipFromIventory;
    }

    private void equipFromIventory(Item item)
    {
        if(item is Equippableitem)
        {
            equip((Equippableitem)item);
        }
    }

    public void equip(Equippableitem equippableitem)
    {
        if(inventory.removeItem(equippableitem))
        {
            Equippableitem previusItem;
            if(equipmentPanel.addItem(equippableitem, out previusItem))
            {
                if(previusItem)
                {
                    inventory.addItem(previusItem);
                }
            }
            else
            {
                inventory.addItem(equippableitem);
            }
        }
    }

    public void unequip(Equippableitem equippableitem )
    {
        if(!inventory.isFull() && equipmentPanel.removeItem(equippableitem))
        {
            inventory.addItem(equippableitem);
        }
    }
}
