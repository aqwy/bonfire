using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _imageItem;

    public event Action<Item> onRightClickEvent;

    private Item _Item;
    public Item Item
    {
        get
        {
            return _Item;
        }
        set
        {
            _Item = value;
            if (!_Item)
            {
                _imageItem.enabled = false;
            }
            else
            {
                _imageItem.sprite = _Item.icon;
                _imageItem.enabled = true;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null && onRightClickEvent != null)
            {
                onRightClickEvent(Item);
            }
        }
    }

    protected virtual void OnValidate()
    {
        if (!_imageItem)
        {
            _imageItem = GetComponent<Image>();
        }
    }
}
