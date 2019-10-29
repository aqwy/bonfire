using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> items;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private ItemSlot[] itemSlots;

    public event Action<Item> onItemRightClickedEvent;

    private void Awake()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].onRightClickEvent += onItemRightClickedEvent;
        }
    }

    private void OnValidate()
    {
        if (itemsParent)
        {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
            refreshUI();
        }
    }

    private void refreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = items[i];
        }

        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = null;
        }
    }

    public bool addItem(Item newItem)
    {
        if (isFull())
            return false;

        items.Add(newItem);
        refreshUI();
        return true;
    }

    public bool removeItem(Item delItem)
    {
        if (items.Remove(delItem))
        {
            refreshUI();
            return true;
        }

        return false;
    }

    public bool isFull()
    {
        return items.Count >= itemSlots.Length;
    }
}
