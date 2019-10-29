using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventoryManager : Singltone<InventoryManager>
{
    [SerializeField] private Inventory inventory;
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

    /*public List<Image> inventoryImgs;
    public Image activeImg;

    private List<string> _items;

    void Start()
    {
        if (!activeImg.sprite)
        {
            setAlpha(0f);
        }

        if (activeImg.sprite)
        {
            inventoryImgs.Add(activeImg);
            setAlpha(1f);
        }
    }

    void Update()
    {

    }

    private void setAlpha(float alpha)
    {
        Color tempAlfaColor = activeImg.color;
        tempAlfaColor.a = alpha;
        activeImg.color = tempAlfaColor;
    }

    public void swapSprites(Image swapingImg)
    {
        bool findSpace = false;

        if (!activeImg.sprite)
        {
            Debug.Log("active img not found ");
            activeImg.sprite = swapingImg.sprite;

            setAlpha(1f);

        }
        else if (activeImg.sprite)
        {
            Debug.Log("active img found ");
            for (int i = 0; i < inventoryImgs.Count; i++)
            {             
                if (!inventoryImgs[i].sprite)
                {
                    Debug.Log("find empty slot");
                    Sprite temp = activeImg.sprite;
                    activeImg.sprite = swapingImg.sprite;
                    inventoryImgs[i].sprite = temp;
                    findSpace = true;
                }
            }

            if(!findSpace)
            {
                / *var aga = inventoryImgs.FindIndex();
                if(aga)
                {
                    Debug.Log("est v kolekcii");
                }* /
                Sprite tempSwipping = swapingImg.sprite;
                Debug.Log("cant find empty slot");
            }
        }
    }
    public void addItem(string itemName)
    {
        _items.Add(itemName);
    }

    private void displayItems()
    {
        string itemDisplay = "Items: ";
        foreach (string item in _items)
        {
            itemDisplay += item + " ";
        }

        Debug.Log(itemDisplay);
    }*/

}
