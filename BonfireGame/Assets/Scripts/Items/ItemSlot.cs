using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ItemSlot : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image _imageItem;

    public event Action<Item> onRightClickEvent;

    protected Color normalColor = Color.white;
    protected Color disabledColor = new Color(1, 1, 1, 0);

    private Outline _outline;
    private Vector2 _imageScale;

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
                _imageItem.sprite = null;
                _imageItem.color = disabledColor;
            }

            else
            {
                _imageItem.sprite = _Item.icon;
                _imageItem.color = normalColor;
            }
        }
    }
    private void Start()
    {
        _outline = GetComponent<Outline>();
        _imageScale = _imageItem.rectTransform.localScale;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null && onRightClickEvent != null)
            {
                onRightClickEvent(Item);
                GameManager.Instance.setInteractive();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _imageItem.rectTransform.localScale = new Vector2(1.2f, 1.2f);
        _outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _imageItem.rectTransform.localScale = _imageScale;
        _outline.enabled = false;
    }
}
