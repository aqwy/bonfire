using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HummerUpgrade : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SpriteRenderer interactFrameSprite;
    public Equippableitem newSuperHummer;

    private Equippableitem _currItem;
    private void Start()
    {
        interactFrameSprite.enabled = false;
    }
    public void startHummUpgrade()
    {
        InventoryManager.Instance.inventory.addItem(newSuperHummer);
        UIManager.Instance.newItemSignPanel.gameObject.SetActive(true);
        GameManager.Instance.addWinKey();
        Debug.Log("upgraeing item");
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("humer click");
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (_currItem.effecttype.ToString() == "activator")
            {
                var itemtoRemove = GameManager.Instance.activeItem;
                itemtoRemove.removeItem(_currItem);
                gameObject.SetActive(false);
                /*GetComponent<SpriteRenderer>().enabled = false;
                interactFrameSprite.enabled = false;*/
                startHummUpgrade();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (_currItem.effecttype.ToString() == "activator")
            {
                interactFrameSprite.enabled = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactFrameSprite.enabled = false;
    }
}
