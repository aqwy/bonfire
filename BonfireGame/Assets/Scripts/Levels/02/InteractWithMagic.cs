using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractWithMagic : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    public SpriteRenderer interactFrameSprite;
    public SpriteRenderer magicHammer;
    public MagicBottle magicBottle;

    private bool _canUpdate = false;
    private Equippableitem _currItem;

    private void Start()
    {
        interactFrameSprite.enabled = false;
        magicHammer.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        /*_canUpdate = magicBottle.canBottleUse();
        if (_canUpdate)
        {
            _currItem = GameManager.Instance.activeItem.getCurrentItem();
            if (_currItem)
            {
                if (_currItem.effecttype.ToString() == "crush")
                {
                    UIManager.Instance.gameplayElements.SetActive(false);
                    var itemtoRemove = GameManager.Instance.activeItem;
                    itemtoRemove.removeItem(_currItem);
                    magicHammer.enabled = true;
                    magicHammer.gameObject.layer = 0;
                    interactFrameSprite.enabled = false;
                    this.gameObject.GetComponent<InteractWithMagic>().enabled = false;
                }
            }
        }*/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canUpdate = magicBottle.canBottleUse();
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (_canUpdate && _currItem.effecttype.ToString() == "crush")
            {
                UIManager.Instance.gameplayElements.SetActive(false);
                interactFrameSprite.enabled = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactFrameSprite.enabled = false;

        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if(_currItem)
        {
            UIManager.Instance.gameplayElements.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _canUpdate = magicBottle.canBottleUse();
        if (_canUpdate)
        {
            _currItem = GameManager.Instance.activeItem.getCurrentItem();
            if (_currItem)
            {
                if (_currItem.effecttype.ToString() == "crush")
                {
                    UIManager.Instance.gameplayElements.SetActive(false);
                    var itemtoRemove = GameManager.Instance.activeItem;
                    itemtoRemove.removeItem(_currItem);
                    magicHammer.enabled = true;
                    magicHammer.gameObject.layer = 0;
                    interactFrameSprite.enabled = false;
                    this.gameObject.GetComponent<InteractWithMagic>().enabled = false;
                }
            }
        }
    }
}
